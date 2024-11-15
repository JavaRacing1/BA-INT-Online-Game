using System.Collections.Generic;

using Godot;

using INTOnlineCoop.Script.Level;
using INTOnlineCoop.Script.Singleton;
using INTOnlineCoop.Script.UI.Component;
using INTOnlineCoop.Script.Player;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// Screen for configuring the sandbox mode
    /// </summary>
    public partial class LobbyScreen : Control
    {
        [Export] private GeneratorSettingsContainer _generatorSettings;
        [Export] private Container _generatorContainer;
        [Export] private Container _playerInformationContainer;
        [Export] private Button _playButton;

        private GameConfirmationDialog _quitDialog;
        private int _currentPlayerIndex;

        /// <summary>
        /// Generate player information UI
        /// </summary>
        public override void _Ready()
        {
            if (_generatorContainer != null && _playButton != null)
            {
                _generatorContainer.Visible = false;
                _playButton.Disabled = true;
            }

            RebuildUserInterface();
            MultiplayerLobby.Instance.PlayerDataChanged += RebuildUserInterface;
            MultiplayerLobby.Instance.LevelDataReceived += OnLevelImageReceived;
        }

        /// <summary>
        /// Disconnects from PlayerDataChanged signal
        /// </summary>
        public override void _ExitTree()
        {
            MultiplayerLobby.Instance.PlayerDataChanged -= RebuildUserInterface;
            MultiplayerLobby.Instance.LevelDataReceived -= OnLevelImageReceived;
        }

        private void RebuildUserInterface()
        {
            if (_playerInformationContainer == null || !IsInstanceValid(_playerInformationContainer) ||
                _generatorContainer == null || !IsInstanceValid(_generatorContainer))
            {
                return;
            }

            RebuildPlayerInformation();

            int playerDataCount = MultiplayerLobby.Instance.GetPlayerDataCount();
            _generatorContainer.Visible = _currentPlayerIndex == 1 ||
                                          (_currentPlayerIndex == 2 && playerDataCount == 1);
            _playButton.Disabled = _currentPlayerIndex != 1 || playerDataCount == 1;
        }

        private void RebuildPlayerInformation()
        {
            foreach (Node child in _playerInformationContainer.GetChildren())
            {
                child.QueueFree();
            }

            Dictionary<int, PlayerData> playerDataDictionary = new();
            for (int i = 1; i <= MultiplayerLobby.MaxPlayers; i++)
            {
                playerDataDictionary.Add(i, new PlayerData());
            }

            foreach (PlayerData data in MultiplayerLobby.Instance.GetPlayerData())
            {
                if (data.PlayerNumber == -1)
                {
                    continue;
                }

                if (data == MultiplayerLobby.Instance.CurrentPlayerData)
                {
                    _currentPlayerIndex = data.PlayerNumber;
                }

                playerDataDictionary[data.PlayerNumber] = data;
            }

            PackedScene informationItemScene =
                (PackedScene)ResourceLoader.Load("res://scene/ui/component/PlayerInformationItem.tscn");
            foreach (KeyValuePair<int, PlayerData> data in playerDataDictionary)
            {
                PlayerInformationItem item = informationItemScene.Instantiate<PlayerInformationItem>();
                item.SetPlayerNumber(data.Key);
                item.SetPlayerName(data.Value.Name);
                _playerInformationContainer.AddChild(item);
            }
        }

        private void OnBackButtonPressed()
        {
            if (_quitDialog == null)
            {
                _quitDialog = new("Verbindung trennen", "Möchtest du wirklich die Verbindung zum Server trennen?");
                _quitDialog.GetOkButton().Pressed += () =>
                {
                    MultiplayerLobby.Instance.CloseConnection();
                    _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/MainMenu.tscn");
                };
                AddChild(_quitDialog);
            }

            _quitDialog.Visible = true;
        }

        private void OnPlayButtonPressed()
        {
            if (_generatorSettings == null || _playButton == null)
            {
                return;
            }
            MultiplayerLobby.Instance.SendGeneratorSettingsToServer(_generatorSettings.SelectedTerrainShape,
                _generatorSettings.Seed);
            _playButton.Disabled = true;
        }

        private void OnLevelImageReceived(Image levelImage)
        {
            GameLevel level = GD.Load<PackedScene>("res://scene/level/GameLevel.tscn").Instantiate<GameLevel>();
            level.Init(levelImage);
            GetTree().Root.AddChild(level);
            GetTree().CurrentScene = level;
            QueueFree();
        }
    }
}
