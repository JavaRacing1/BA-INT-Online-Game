using Godot;

using INTOnlineCoop.Script.UI.Component;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// The start scene of the game
    /// </summary>
    public partial class MainMenu : Control
    {
        private const string CreditFilePath = "res://config/credits.txt";
        private GameConfirmationDialog _exitDialog;
        private SettingsWindow _settingsWindow;
        private CanvasLayer _creditCanvas;
        private MultiplayerConnectionWindow _connectionWindow;

        /// <summary>
        /// Receives node references and loads the credit file
        /// </summary>
        public override void _Ready()
        {
            _creditCanvas = GetNode<CanvasLayer>("CreditLayer");
            LoadCredits();
        }

        private void LoadCredits()
        {
            if (!FileAccess.FileExists(CreditFilePath))
            {
                return;
            }

            RichTextLabel creditTextLabel = GetNode<RichTextLabel>("%CreditTextLabel");
            using FileAccess creditFile = FileAccess.Open(CreditFilePath, FileAccess.ModeFlags.Read);
            if (creditFile == null)
            {
                GD.PrintErr($"File Error: {FileAccess.GetOpenError()}");
                return;
            }

            string text = creditFile.GetAsText();
            creditTextLabel.Text = text;
        }

        private void OnExitButtonPressed()
        {
            if (_exitDialog == null)
            {
                _exitDialog = new("Spiel verlassen", "Möchtest du wirklich das Spiel verlassen?");
                _exitDialog.GetOkButton().Pressed += () => GetTree().Quit();
                AddChild(_exitDialog);
            }

            _exitDialog.Visible = true;
        }

        private void OnSettingsButtonPressed()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = GD.Load<PackedScene>("res://scene/ui/screen/SettingsWindow.tscn")
                    .Instantiate<SettingsWindow>();
                AddChild(_settingsWindow);
            }

            _settingsWindow.Visible = true;
        }

        private void OnCreditsButtonPressed()
        {
            _creditCanvas.Visible = true;
        }

        private void OnCloseCreditsButtonPressed()
        {
            _creditCanvas.Visible = false;
        }

        private void OnSandboxButtonPressed()
        {
            _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/SandboxSettingsScreen.tscn");
        }

        private void OnMultiplayerButtonPressed()
        {
            if (_connectionWindow == null)
            {
                _connectionWindow = GD.Load<PackedScene>("res://scene/ui/component/MultiplayerConnectionWindow.tscn")
                    .Instantiate<MultiplayerConnectionWindow>();
                AddChild(_connectionWindow);
            }

            _connectionWindow.Visible = true;
        }

        private void OnCharacterSelectionButtonPressed()
        {
            _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/CharacterSelection.tscn");
        }

        private void OnIntroductionButtonPressed()
        {
            _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/GameTutorial.tscn");
        }
    }
}