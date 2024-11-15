using Godot;

using INTOnlineCoop.Script.Player;

namespace INTOnlineCoop.Script.UI.Component
{
    /// <summary>
    /// Component used for the character selection
    /// </summary>
    public partial class CharacterSelectionItem : Button
    {
        [Export] private TextureRect _headTexture;
        [Export] private Label _nameLabel;

        private bool _isSelected;

        /// <summary>
        /// Emitted when the character is selected/unselected
        /// </summary>
        [Signal]
        public delegate void SelectedCharacterChangedEventHandler(bool isSelected, CharacterType type);

        /// <summary>
        /// Character represented by the component
        /// </summary>
        public CharacterType CharacterType { get; private set; }

        /// <summary>
        /// Changes the character type of the component
        /// </summary>
        /// <param name="characterType"></param>
        public void SetCharacterType(CharacterType characterType)
        {
            CharacterType = characterType;
            if (_nameLabel == null || _headTexture == null)
            {
                return;
            }

            _nameLabel.Text = characterType.Name;
            _headTexture.Texture = characterType.HeadTexture;
        }

        /// <summary>
        /// Changes the disabled status + label
        /// </summary>
        /// <param name="isDisabled">True if button should be disabled</param>
        public void DisableButton(bool isDisabled)
        {
            Disabled = isDisabled;
            if (_nameLabel == null)
            {
                return;
            }

            _nameLabel.AddThemeColorOverride("font_color",
                isDisabled ? new Color(70 / 255f, 70 / 255f, 70 / 255f) : new Color(1.0f, 1.0f, 1.0f));
        }

        private void OnButtonPressed()
        {
            if (_nameLabel == null)
            {
                return;
            }

            _isSelected = !_isSelected;
            _nameLabel.AddThemeColorOverride("font_color",
                _isSelected ? new Color(0, 1.0f, 0) : new Color(1.0f, 1.0f, 1.0f));
            Error error = EmitSignal(SignalName.SelectedCharacterChanged, _isSelected, CharacterType);
            if (error != Error.Ok)
            {
                GD.PrintErr("Could not emit SelectedCharacterChanged event: " + error);
            }
        }
    }
}