using System;

using Godot;

using INTOnlineCoop.Script.Singleton;
using INTOnlineCoop.Script.Player;

namespace INTOnlineCoop.Script.UI.Component
{
    /// <summary>
    /// Window for server connection settings
    /// </summary>
    public partial class MultiplayerConnectionWindow : Window
    {
        [Export] private LineEdit _usernameInput;
        [Export] private LineEdit _serverAddressInput;
        [Export] private LineEdit _portInput;
        [Export] private Button _connectionButton;
        [Export] private Label _errorLabel;

        /// <summary>
        /// Connects to PlayerConnected signal
        /// </summary>
        public override void _Ready()
        {
            MultiplayerLobby.Instance.PlayerConnected += OnPlayerConnected;
        }

        /// <summary>
        /// Disconnects from PlayerConnected signal
        /// </summary>
        public override void _ExitTree()
        {
            MultiplayerLobby.Instance.PlayerConnected -= OnPlayerConnected;
        }

        private void OnCloseRequested()
        {
            Visible = false;
        }

        private void OnConnectButtonPressed()
        {
            if (_usernameInput == null || _serverAddressInput == null || _portInput == null)
            {
                return;
            }
            //Reset error display
            DisplayError("");

            string username = _usernameInput.Text;
            if (username == "")
            {
                username = GenerateUsername();
            }

            MultiplayerLobby.Instance.CreatePlayerData(username);

            string serverAddress = _serverAddressInput.Text;
            string portString = _portInput.Text;

            Error connectionError;
            if (portString != "")
            {
                try
                {
                    int port = Convert.ToInt32(portString);
                    connectionError = MultiplayerLobby.Instance.JoinGame(DisplayTimeoutError, serverAddress, port);
                }
                catch (Exception e)
                {
                    GD.PrintErr("Port is not a valid number: " + e.Message);
                    DisplayError("Fehler: Port ist keine gültige Zahl!");
                    return;
                }
            }
            else
            {
                connectionError = MultiplayerLobby.Instance.JoinGame(DisplayTimeoutError, serverAddress);
            }

            if (connectionError != Error.Ok)
            {
                GD.PrintErr("Server connection failed: " + connectionError);
                DisplayError("Fehler beim Verbinden: " + connectionError);
            }
            else if (_connectionButton != null)
            {
                // Disable button to prevent new connections during waiting
                _connectionButton.Disabled = true;
            }
        }

        private string DisplayTimeoutError()
        {
            DisplayError("Fehler beim Verbinden: Timeout");
            if (_connectionButton != null)
            {
                _connectionButton.Disabled = false;
            }

            return "Timeout";
        }

        private void DisplayError(string message)
        {
            if (_errorLabel != null)
            {
                _errorLabel.Text = message;
            }
        }

        private void OnPlayerConnected(int peerId, PlayerData playerData)
        {
            if (MultiplayerLobby.Instance.CurrentPlayerData != playerData)
            {
                return;
            }

            Error error = GetTree().ChangeSceneToFile("res://scene/ui/screen/LobbyScreen.tscn");
            if (error != Error.Ok)
            {
                GD.PrintErr("Error during loading of LobbyScreen: " + error);
            }
        }

        private static string GenerateUsername()
        {
            string username = "";
            Random random = new();
            for (int i = 0; i < 6; i++)
            {
                username += random.Next(10).ToString();
            }

            return username;
        }
    }
}
