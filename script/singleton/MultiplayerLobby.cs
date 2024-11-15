using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Godot;

using INTOnlineCoop.Script.Level;
using INTOnlineCoop.Script.Player;

namespace INTOnlineCoop.Script.Singleton
{
    /// <summary>
    /// Singleton node for managing multiplayer connections
    /// </summary>
    public partial class MultiplayerLobby : Node
    {
        /// <summary>
        /// Maximum player amount
        /// </summary>
        public const int MaxPlayers = 2;

        private const string DefaultIp = "127.0.0.1";
        private const int DefaultPort = 7070;

        private readonly Dictionary<long, PlayerData> _playerData = new();
        private readonly SortedSet<int> _freePlayerNumbers = new() { 1, 2 };

        /// <summary>
        /// Current MultiplayerLobby instance
        /// </summary>
        public static MultiplayerLobby Instance { get; private set; }

        /// <summary>
        /// Local player data
        /// </summary>
        public PlayerData CurrentPlayerData { get; private set; }

        /// <summary>
        /// Signal emitted after a successful player connection
        /// </summary>
        [Signal]
        public delegate void PlayerConnectedEventHandler(int peerId, PlayerData playerInfo);

        /// <summary>
        /// Signal emitted after a player disconnects
        /// </summary>
        [Signal]
        public delegate void PlayerDisconnectedEventHandler(int peerId);

        /// <summary>
        /// Signal emitted after the server disconnected
        /// </summary>
        [Signal]
        public delegate void ServerDisconnectedEventHandler();

        /// <summary>
        /// Signal emitted after the player received its player number
        /// </summary>
        [Signal]
        public delegate void PlayerReceivedNumberEventHandler(int peerId, int playerNumber);

        /// <summary>
        /// Signal emitted after saved player data changed
        /// </summary>
        [Signal]
        public delegate void PlayerDataChangedEventHandler();

        /// <summary>
        /// Signal emitted after client received level image from server
        /// </summary>
        [Signal]
        public delegate void LevelDataReceivedEventHandler(Image levelImage);

        /// <summary>
        /// Initializes the lobby node + starts the server
        /// </summary>
        public override void _Ready()
        {
            Instance = this;

            if (DisplayServer.GetName() == "headless" || OS.HasFeature("dedicated_server"))
            {
                int serverPort = DefaultPort;
                //Use custom port from CMD if available
                string[] cmdArgs = OS.GetCmdlineArgs();
                foreach (string arg in cmdArgs)
                {
                    if (!arg.Contains('=') || !arg.Contains("port"))
                    {
                        continue;
                    }

                    string inputPort = arg.Split('=').Last();
                    try
                    {
                        serverPort = Convert.ToInt32(inputPort);
                    }
                    catch (Exception)
                    {
                        GD.PrintErr("Invalid server port: " + inputPort);
                    }
                }

                CreateServer(serverPort);
            }

            Multiplayer.PeerConnected += OnPlayerConnected;
            Multiplayer.PeerDisconnected += OnPlayerDisconnected;
            Multiplayer.ConnectedToServer += OnConnectOk;
            Multiplayer.ConnectionFailed += OnConnectionFail;
            Multiplayer.ServerDisconnected += OnServerDisconnected;
        }

        /*
         * =====================================================================================================
         *                                         CONNECTION CREATION
         * =====================================================================================================
         */

        /// <summary>
        /// Creates a connection to the specified server
        /// </summary>
        /// <param name="errorFunc">Function called when the connections fails</param>
        /// <param name="address">Address of the server. Defaults to "127.0.0.1"</param>
        /// <param name="port">Port used by the server. Defaults to "7070"</param>
        /// <returns></returns>
        public Error JoinGame(Func<string> errorFunc, string address = "", int port = DefaultPort)
        {
            _playerData.Clear();
            if (CurrentPlayerData == null)
            {
                GD.PrintErr("Can't connect without local player data!");
                return Error.Failed;
            }

            GD.Print($"Initializing connection with username {CurrentPlayerData.Name}");

            if (string.IsNullOrEmpty(address))
            {
                address = DefaultIp;
            }

            ENetMultiplayerPeer clientPeer = new();
            Error error = clientPeer.CreateClient(address, port);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to create client: " + error);
                return error;
            }

            //Reduce timeout for server connections
            clientPeer.GetPeer(1).SetTimeout(0, 0, 5000);

            Multiplayer.MultiplayerPeer = clientPeer;
            Multiplayer.ConnectionFailed += () => errorFunc.Invoke();
            return Error.Ok;
        }

        /// <summary>
        /// Closes the current multiplayer connection
        /// </summary>
        public void CloseConnection()
        {
            Multiplayer.MultiplayerPeer.DisconnectPeer(1);
            Multiplayer.MultiplayerPeer = null;
            _playerData.Clear();
            CurrentPlayerData = null;
        }

        /// <summary>
        /// Creates a new server
        /// </summary>
        /// <param name="port">Port of the server</param>
        private void CreateServer(int port)
        {
            ENetMultiplayerPeer serverPeer = new();
            Error error = serverPeer.CreateServer(port, MaxPlayers);

            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to create server");
                GD.PrintErr(error.ToString());
            }

            GD.Print($"Server successfully created on port {port}");
            Multiplayer.MultiplayerPeer = serverPeer;
        }

        /*
         * =====================================================================================================
         *                                      PLAYER DATA SYNC
         * =====================================================================================================
         */

        /// <summary>
        /// Creates a new local player data instance
        /// </summary>
        /// <param name="username">Username of the player</param>
        public void CreatePlayerData(string username)
        {
            CurrentPlayerData = new PlayerData { Name = username };
        }

        /// <summary>
        /// Returns an immutable set of PlayerData
        /// </summary>
        /// <returns>Immutable sorted set with PlayerData</returns>
        public ImmutableSortedSet<PlayerData> GetPlayerData()
        {
            return _playerData.Values.ToImmutableSortedSet(Comparer<PlayerData>
                .Create((p1, p2) => p1.PlayerNumber - p2.PlayerNumber));
        }

        /// <summary>
        /// Returns the count of PlayerData
        /// </summary>
        /// <returns>PlayerData count</returns>
        public int GetPlayerDataCount()
        {
            return _playerData.Count;
        }


        /// <summary>
        /// Called from other players with their PlayerData
        /// </summary>
        /// <param name="newPlayerInfo">Serialized data of the sender</param>
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void RegisterPlayer(Godot.Collections.Dictionary<string, Variant> newPlayerInfo)
        {
            int playerId = Multiplayer.GetRemoteSenderId();
            PlayerData playerData = PlayerData.Deserialize(newPlayerInfo);
            if (_playerData.TryGetValue(playerId, out PlayerData savedPlayerData))
            {
                savedPlayerData.UpdateInstance(playerData);
            }
            else
            {
                _playerData[playerId] = playerData;
            }

            Error changedError = EmitSignal(SignalName.PlayerDataChanged);
            if (changedError != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerDataChanged signal: " + changedError);
            }

            GD.Print($"{Multiplayer.GetUniqueId()}: Received PlayerData from {playerId}");
            GD.Print($"{Multiplayer.GetUniqueId()}: {playerId} is {playerData.Name}");

            Error error = EmitSignal(SignalName.PlayerConnected, playerId, playerData);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerConnected signal: " + error);
            }

            if (Multiplayer.IsServer())
            {
                int playerNumber = _freePlayerNumbers.First();
                _ = _freePlayerNumbers.Remove(playerNumber);
                //Send new player number to all peers
                Error rpcError = Rpc(MethodName.SetPlayerNumber, playerId, playerNumber);
                if (rpcError != Error.Ok)
                {
                    GD.PrintErr("Failed to send SetPlayerNumber RPC: " + rpcError);
                }
            }
        }

        /// <summary>
        /// Sets the player number for a peer
        /// </summary>
        /// <param name="peerId">ID of the player peer</param>
        /// <param name="playerNumber">New number of the player</param>
        [Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void SetPlayerNumber(int peerId, int playerNumber)
        {
            if (_playerData.TryGetValue(peerId, out PlayerData savedPlayerData))
            {
                savedPlayerData.PlayerNumber = playerNumber;

                if (Multiplayer.GetUniqueId() == peerId)
                {
                    CurrentPlayerData = savedPlayerData;
                }
            }
            else
            {
                _playerData[peerId] = new PlayerData { PlayerNumber = playerNumber };
            }

            Error changedError = EmitSignal(SignalName.PlayerDataChanged);
            if (changedError != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerDataChanged signal: " + changedError);
            }

            GD.Print($"{Multiplayer.GetUniqueId()}: Received PlayerNumber for {peerId}: {playerNumber}");
        }

        /*
         * =====================================================================================================
         *                                      CLIENT + SERVER SIGNALS
         * =====================================================================================================
         */

        /// <summary>
        /// Called on server + clients when a new player connects to the server
        /// </summary>
        /// <param name="peerId">ID of the new peer</param>
        private void OnPlayerConnected(long peerId)
        {
            if (!Multiplayer.IsServer())
            {
                if (CurrentPlayerData == null)
                {
                    GD.PrintErr("Failed to send PlayerData: Data is null!");
                    return;
                }

                GD.Print($"{Multiplayer.GetUniqueId()}: Received PlayerConnected from {peerId}");

                //Send local PlayerData to new player
                Error error = RpcId(peerId, MethodName.RegisterPlayer, PlayerData.Serialize(CurrentPlayerData));
                if (error != Error.Ok)
                {
                    GD.PrintErr("Failed to send RPC: " + error);
                }
            }
            else
            {
                GD.Print($"{Multiplayer.GetUniqueId()}: Received PlayerConnected on server");
            }
        }

        /// <summary>
        /// Called on server + clients when a player disconnects
        /// </summary>
        /// <param name="peerId">ID of the disconnected player</param>
        private void OnPlayerDisconnected(long peerId)
        {
            GD.Print($"{Multiplayer.GetUniqueId()}: Received PlayerDisconnected from {peerId}");
            int oldPlayerNumber = _playerData[peerId].PlayerNumber;
            _ = _playerData.Remove(peerId);

            Error changedError = EmitSignal(SignalName.PlayerDataChanged);
            if (changedError != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerDataChanged signal: " + changedError);
            }

            Error error = EmitSignal(SignalName.PlayerDisconnected, peerId);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerDisconnected signal: " + error);
            }

            if (Multiplayer.MultiplayerPeer != null && Multiplayer.IsServer())
            {
                _ = _freePlayerNumbers.Add(oldPlayerNumber);
                GD.Print($"{Multiplayer.GetUniqueId()}: Received PlayerDisconnected on server");
            }
        }


        /*
         * =====================================================================================================
         *                                        CLIENT ONLY SIGNALS
         * =====================================================================================================
         */

        /// <summary>
        /// Called when local client connection to the server was successful
        /// </summary>
        private void OnConnectOk()
        {
            int peerId = Multiplayer.GetUniqueId();
            _playerData[peerId] = CurrentPlayerData;
            GD.Print($"{peerId}: Connected to server!");
            if (CurrentPlayerData == null)
            {
                GD.PrintErr("Failed to send PlayerConnected signal: Data is null!");
                return;
            }

            Error changedError = EmitSignal(SignalName.PlayerDataChanged);
            if (changedError != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerDataChanged signal: " + changedError);
            }

            Error error = EmitSignal(SignalName.PlayerConnected, peerId, CurrentPlayerData);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send PlayerConnected signal: " + error);
            }
        }

        /// <summary>
        /// Called when local client connection to the server failed
        /// </summary>
        private void OnConnectionFail()
        {
            GD.Print("Connection failed");
            Multiplayer.MultiplayerPeer = null;
        }

        /// <summary>
        /// Called on client when server closed the connection
        /// </summary>
        private void OnServerDisconnected()
        {
            GD.Print("Server disconnected");
            Multiplayer.MultiplayerPeer = null;
            _playerData.Clear();
            Error error = EmitSignal(SignalName.ServerDisconnected);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send ServerDisconnected signal: " + error);
            }
        }

        /*
         * =====================================================================================================
         *                                              LEVEL SYNC
         * =====================================================================================================
         */

        /// <summary>
        /// Sends the selected generator settings to the server
        /// </summary>
        /// <param name="terrainShape">Shape of the server</param>
        /// <param name="seed">Used seed</param>
        public void SendGeneratorSettingsToServer(TerrainShape terrainShape, int seed)
        {
            Error error = Rpc(MethodName.GenerateLevel, terrainShape.ToString(), seed);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send GenerateLevel RPC: " + error);
            }
        }

        /// <summary>
        /// Initiates the image generation
        /// </summary>
        /// <param name="terrainShapeString">Selected terrain shape</param>
        /// <param name="seed">Used seed</param>
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void GenerateLevel(string terrainShapeString, int seed)
        {
            if (!Multiplayer.IsServer())
            {
                return;
            }

            bool parseOk = Enum.TryParse(terrainShapeString, out TerrainShape terrainShape);
            if (!parseOk)
            {
                GD.PrintErr("Failed to parse TerrainShape: " + terrainShapeString);
                return;
            }

            LevelGenerator levelGenerator = new();
            levelGenerator.SetTerrainShape(terrainShape);
            Image image = levelGenerator.Generate(seed);
            Error error = Rpc(MethodName.SendLevelToClient, image.GetWidth(), image.GetHeight(), image.GetData());
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send SendLevelToClient RPC: " + error);
            }
        }

        /// <summary>
        /// Emits a client signal with the level image data
        /// </summary>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <param name="levelImageData">Image data</param>
        [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void SendLevelToClient(int width, int height, byte[] levelImageData)
        {
            if (Multiplayer.IsServer())
            {
                return;
            }

            Image levelImage = Image.CreateFromData(width, height, false, Image.Format.Rgba8, levelImageData);
            Error error = EmitSignal(SignalName.LevelDataReceived, levelImage);
            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to send LevelDataReceived signal: " + error);
            }
        }
    }
}