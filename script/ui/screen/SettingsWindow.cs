using Godot;

namespace OnlineGame
{
    /// <summary>
    /// Screen for the user to change his settings
    /// </summary>
    public partial class SettingsWindow : CanvasLayer
    {
        private void OnCancelButtonPressed()
        {
            Visible = false;
        }

        private void OnDiscardButtonPressed()
        {
            //TODO: Reset unsaved changes
        }

        private void OnApplyButtonPressed()
        {
            //TODO: Apply changes
            Visible = false;
        }
    }
}

