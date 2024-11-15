using System.Collections.Generic;

using Godot;

using INTOnlineCoop.Script.Player;
using INTOnlineCoop.Script.UI.Component;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// Scene to select player characters (4 for each player, no repeat of characters)
    /// </summary>
    public partial class CharacterSelection : Node
    {
        // Verweise auf Bestätigen und Abbrechen-Buttons
        [Export] private TextureButton _confirmSelectionButton;
        [Export] private TextureButton _returnSelectionButton;
        [Export] private GridContainer _characterGrid;

        //PopUp verstecken
        private GameConfirmationDialog _exitDialog;

        // Maximal vier Figuren auswählbar
        private const int MaxSelections = 4;

        /// <summary>
        /// Liste der Spielfiguren von User
        /// </summary>
        public List<CharacterType> SelectedCharacters { get; } = new();

        /// <summary>
        /// Initialize all label nodes with the text color "white",
        /// deactivate the "confirmSelectionButton" button and make it invisible
        /// </summary>
        public override void _Ready()
        {
            // Setze den Anfangszustand für die Buttons
            _confirmSelectionButton.Visible = false;

            if (_characterGrid != null)
            {
                PackedScene itemScene = GD.Load<PackedScene>("res://scene/ui/component/CharacterSelectionItem.tscn");
                foreach (CharacterType type in CharacterType.Values)
                {
                    CharacterSelectionItem item = itemScene.Instantiate<CharacterSelectionItem>();
                    item.SetCharacterType(type);
                    item.SelectedCharacterChanged += OnSelectedCharacterChanged;
                    _characterGrid.AddChild(item);
                    if (type == CharacterType.Gaige || type == CharacterType.Nisha || type == CharacterType.Zero)
                    {
                        continue;
                    }

                    Control spacer = new()
                    {
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                        SizeFlagsVertical = Control.SizeFlags.ExpandFill
                    };
                    _characterGrid.AddChild(spacer);
                }
            }

            //Überprüfe funktionlität ConfirmSelectionButton
            CheckSelectedCharacters();
            UpdateButtons();
        }

        private void OnSelectedCharacterChanged(bool isSelected, CharacterType characterType)
        {
            if (isSelected && SelectedCharacters.Count < MaxSelections)
            {
                SelectedCharacters.Add(characterType);
            }
            else
            {
                _ = SelectedCharacters.Remove(characterType);
            }

            CheckSelectedCharacters();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (_characterGrid == null)
            {
                return;
            }

            bool isDisabled = SelectedCharacters.Count >= MaxSelections;

            foreach (Node node in _characterGrid.GetChildren())
            {
                if (node is CharacterSelectionItem characterSelectionItem &&
                    !SelectedCharacters.Contains(characterSelectionItem.CharacterType))
                {
                    characterSelectionItem.DisableButton(isDisabled);
                }
            }
        }

        /// <summary>
        /// Üperprüfe Anzahl ausgewählter Spielfiguren, aktiviere ConfirmSelectionButton
        /// </summary>
        private void CheckSelectedCharacters()
        {
            _confirmSelectionButton.Visible = SelectedCharacters.Count == MaxSelections;
        }

        /// <summary>
        /// zeige PopUp
        /// </summary>
        private void OnCharacterSelectionConfirmPressed()
        {
            _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/MainMenu.tscn");
        }

        /// <summary>
        /// wechsel zurück auf Main-Szene
        /// </summary>
        private void OnCharacterSelectReturnPressed()
        {
            if (_exitDialog == null)
            {
                _exitDialog = new GameConfirmationDialog("Auswahl verlassen",
                    "Möchtest du die Charakterauswahl abbrechen?");
                _exitDialog.GetOkButton().Pressed += () =>
                    _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/MainMenu.tscn");
                AddChild(_exitDialog);
            }

            _exitDialog.Visible = true;
        }
    };
}