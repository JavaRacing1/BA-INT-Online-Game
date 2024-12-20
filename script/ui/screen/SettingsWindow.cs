using System;

using Godot;

using INTOnlineCoop.Script.Singleton;
using INTOnlineCoop.Script.UI.Component;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// Screen for the user to change his settings
    /// </summary>
    public partial class SettingsWindow : CanvasLayer
    {
        [Export] private OptionButton _displayModeButton;
        [Export] private CheckBox _particleCheckBox;
        [Export] private HSlider _masterVolumeSlider;
        [Export] private HSlider _musicVolumeSlider;
        [Export] private HSlider _effectVolumeSlider;
        [Export] private Label _masterCurrentVolumeLabel;
        [Export] private Label _musicCurrentVolumeLabel;
        [Export] private Label _effectCurrentVolumeLabel;
        [Export] private CheckBox _controlHintCheckBox;
        [Export] private VBoxContainer _inputContainer;

        private GameConfirmationDialog _cancelDialog;
        private GameConfirmationDialog _discardDialog;
        private InputConfigItem _selectedConfigItem;
        private Button _selectedButton;
        private string _selectedAction;
        private InputKind _selectedInputKind;
        private bool _outdatedControls;

        /// <summary>
        /// Initializes the settings window
        /// </summary>
        public override void _Ready()
        {
            if (_displayModeButton != null)
            {
                _displayModeButton.ItemSelected += index => PlayerSettingsData.Instance.SetDisplayMode((DisplayMode)index);
            }

            if (_particleCheckBox != null)
            {
                _particleCheckBox.Toggled += toggled => PlayerSettingsData.Instance.SetParticlesEnabled(toggled);
            }

            if (_masterVolumeSlider != null && _masterCurrentVolumeLabel != null)
            {
                _masterVolumeSlider.ValueChanged += volume =>
                {
                    PlayerSettingsData.Instance.SetMasterVolume((int)volume);
                    _masterCurrentVolumeLabel.Text = Convert.ToString((int)volume);
                };
            }

            if (_musicVolumeSlider != null && _musicCurrentVolumeLabel != null)
            {
                _musicVolumeSlider.ValueChanged += volume =>
                {
                    PlayerSettingsData.Instance.SetMusicVolume((int)volume);
                    _musicCurrentVolumeLabel.Text = Convert.ToString((int)volume);
                };
            }

            if (_effectVolumeSlider != null && _effectCurrentVolumeLabel != null)
            {
                _effectVolumeSlider.ValueChanged += volume =>
                {
                    PlayerSettingsData.Instance.SetEffectVolume((int)volume);
                    _effectCurrentVolumeLabel.Text = Convert.ToString((int)volume);
                };
            }

            if (_controlHintCheckBox != null)
            {
                _controlHintCheckBox.Toggled += toggled => PlayerSettingsData.Instance.SetControlHintVisibility(toggled);
            }

            _outdatedControls = true;
            UpdateSettings();
        }

        /// <summary>
        /// Listens to inputs of the player
        /// </summary>
        /// <param name="event">The input event</param>
        public override void _Input(InputEvent @event)
        {
            if (_selectedButton == null)
            {
                return;
            }

            string newInput = "";
            InputType newInputType = InputType.Key;

            if (@event is InputEventKey keyEvent)
            {
                newInput = keyEvent.KeyLabel.ToString();
            }
            else if (@event is InputEventJoypadButton buttonEvent)
            {
                newInput = buttonEvent.ButtonIndex.ToString();
                newInputType = InputType.JoyButton;
            }
            else if (@event is InputEventJoypadMotion motionEvent)
            {
                if (motionEvent.AxisValue is < 0.5f and > -0.5f)
                {
                    return;
                }
                char prefix = motionEvent.AxisValue > 0 ? '+' : '-';
                newInput = prefix + motionEvent.Axis.ToString();
                newInputType = InputType.JoyAxis;
            }

            if (newInput != "")
            {
                _selectedConfigItem.ChangeInput((newInput, newInputType), _selectedInputKind);
                _selectedButton.ThemeTypeVariation = "TransparentButton";
                _selectedButton.FocusMode = Control.FocusModeEnum.All;
                _selectedButton = null;
                PlayerSettingsData.Instance.SetInput(_selectedAction, _selectedInputKind, newInput, newInputType);
                GetViewport().SetInputAsHandled();
            }
        }

        private void UpdateSettings()
        {
            if (_displayModeButton != null)
            {
                _displayModeButton.Selected = (int)PlayerSettingsData.Instance.SelectedDisplayMode;
            }

            if (_particleCheckBox != null)
            {
                _particleCheckBox.ButtonPressed = PlayerSettingsData.Instance.AreParticlesEnabled;
            }

            if (_masterVolumeSlider != null && _masterCurrentVolumeLabel != null)
            {
                _masterVolumeSlider.Value = PlayerSettingsData.Instance.MasterVolume;
                _masterCurrentVolumeLabel.Text = Convert.ToString(PlayerSettingsData.Instance.MasterVolume);
            }

            if (_musicVolumeSlider != null && _musicCurrentVolumeLabel != null)
            {
                _musicVolumeSlider.Value = PlayerSettingsData.Instance.MusicVolume;
                _musicCurrentVolumeLabel.Text = Convert.ToString(PlayerSettingsData.Instance.MusicVolume);
            }

            if (_effectVolumeSlider != null && _effectCurrentVolumeLabel != null)
            {
                _effectVolumeSlider.Value = PlayerSettingsData.Instance.EffectVolume;
                _effectCurrentVolumeLabel.Text = Convert.ToString(PlayerSettingsData.Instance.EffectVolume);
            }

            if (_controlHintCheckBox != null)
            {
                _controlHintCheckBox.ButtonPressed = PlayerSettingsData.Instance.ShowControlHints;
            }

            if (_inputContainer != null && _outdatedControls)
            {
                foreach (Node node in _inputContainer.GetChildren())
                {
                    if (node is InputConfigItem)
                    {
                        _inputContainer.RemoveChild(node);
                        node.QueueFree();
                    }
                }

                PackedScene itemScene = GD.Load<PackedScene>("res://scene/ui/component/InputConfigItem.tscn");

                foreach (string action in PlayerSettingsData.InputActions)
                {
                    InputConfigItem item = itemScene.Instantiate<InputConfigItem>();
                    (string, InputType) primaryInput = PlayerSettingsData.Instance.GetInput(action, InputKind.Primary);
                    (string, InputType) secondaryInput = PlayerSettingsData.Instance.GetInput(action, InputKind.Secondary);
                    item.Init(action, primaryInput, secondaryInput);

                    item.InputButtonPressed += (button, kind) => OnInputButtonPressed(item, button, action, Enum.Parse<InputKind>(kind));
                    _inputContainer.AddChild(item);
                }

                _outdatedControls = false;
            }
        }

        private void OnInputButtonPressed(InputConfigItem item, Button button, string action, InputKind inputKind)
        {
            if (_selectedButton != button)
            {
                if (_selectedButton != null)
                {
                    _selectedButton.ThemeTypeVariation = "TransparentButton";
                }

                _selectedConfigItem = item;
                _selectedButton = button;
                _selectedButton.ThemeTypeVariation = "SelectedButton";
                _selectedAction = action;
                _selectedInputKind = inputKind;
                _selectedButton.FocusMode = Control.FocusModeEnum.None;
            }
        }

        private void OnCancelButtonPressed()
        {
            void PressAction()
            {
                _outdatedControls = PlayerSettingsData.Instance.HasControlChanges;
                PlayerSettingsData.Instance.DiscardChanges();
                UpdateSettings();
                Visible = false;
            }

            if (_cancelDialog == null)
            {
                _cancelDialog = new("Einstellungen verlassen",
                    "Möchtest du die Einstellungen wirklich verlassen? Deine Änderungen werden nicht gespeichert");
                _cancelDialog.GetOkButton().Pressed += () => PressAction();
                AddChild(_cancelDialog);
            }

            if (PlayerSettingsData.Instance.HasUnsavedChanges)
            {
                _cancelDialog.Visible = true;
            }
            else
            {
                PressAction();
            }
        }

        private void OnDiscardButtonPressed()
        {
            void PressAction()
            {
                _outdatedControls = PlayerSettingsData.Instance.HasControlChanges;
                PlayerSettingsData.Instance.DiscardChanges();
                UpdateSettings();
            }

            if (_discardDialog == null)
            {
                _discardDialog = new("Änderungen verwerfen",
                    "Möchtest du deine ungespeicherten Änderungen wirklich verwerfen?");
                _discardDialog.GetOkButton().Pressed += () => PressAction();
                AddChild(_discardDialog);
            }

            if (PlayerSettingsData.Instance.HasUnsavedChanges)
            {
                _discardDialog.Visible = true;
            }
            else
            {
                PressAction();
            }
        }

        private void OnApplyButtonPressed()
        {
            PlayerSettingsData.Instance.ApplyChanges();
            Visible = false;
        }
    }
}
