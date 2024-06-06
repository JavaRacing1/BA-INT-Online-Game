using Godot;

using INTOnlineCoop.Script.Level;
using INTOnlineCoop.Script.UI.Screen;

namespace INTOnlineCoop.Script.UI.Component
{
    /// <summary>
    /// Window when a player presses ESC in a level
    /// </summary>
    public partial class PauseDialog : Control
    {
        /// <summary>
        /// Emitted when the exit button is pressed + confirmed
        /// </summary>
        [Signal]
        public delegate void ExitConfirmedEventHandler();

        private GameConfirmationDialog _exitDialog;
        private SettingsWindow _settingsWindow;

        private void OnResumeButtonPressed()
        {
            Visible = false;
            GameLevel.IsInputBlocked = false;
        }

        private void OnSettingsButtonPressed()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = GD.Load<PackedScene>("res://scene/ui/screen/SettingsWindow.tscn")
                    .Instantiate<SettingsWindow>();
                AddChild(_settingsWindow);
            }

            _settingsWindow.Layer = 3;
            _settingsWindow.Visible = true;
        }

        private void OnExitButtonPressed()
        {
            if (_exitDialog == null)
            {
                _exitDialog = new("Level verlassen", "Möchtest du wirklich das Level verlassen?");
                _exitDialog.GetOkButton().Pressed += () => EmitSignal(SignalName.ExitConfirmed);
                AddChild(_exitDialog);
            }

            _exitDialog.Visible = true;
        }
    }
}