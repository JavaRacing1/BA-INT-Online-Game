using System.Collections.Generic;

using Godot;

using INTOnlineCoop.Script.UI.Component;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// Scene to select player characters (4 for each player, no repeat of characters)
    /// </summary>
    public partial class CharacterSelection : Node
    {
        //lable-Nodes von Charakters
        [Export] private TextureButton _headAmaraButton;
        [Export] private TextureButton _headAthenaButton;
        [Export] private TextureButton _headAxtonButton;
        [Export] private TextureButton _headGaigeButton;
        [Export] private TextureButton _headMajaButton;
        [Export] private TextureButton _headMozeButton;
        [Export] private TextureButton _headNishaButton;
        [Export] private TextureButton _headKriegButton;
        [Export] private TextureButton _headSalvadorButton;
        [Export] private TextureButton _headWhilhelmButton;
        [Export] private TextureButton _headZeroButton;

        //TextureButton-Nodes von Charakterss

        [Export] private Label _amaraLabel;
        [Export] private Label _athenaLabel;
        [Export] private Label _axtonLabel;
        [Export] private Label _gaigeLabel;
        [Export] private Label _majaLabel;
        [Export] private Label _mozeLabel;
        [Export] private Label _nishaLabel;
        [Export] private Label _kriegLabel;
        [Export] private Label _salvadorLabel;
        [Export] private Label _whilhelmLabel;
        [Export] private Label _zeroLabel;

        //PopUp verstecken
        private GameConfirmationDialog _exitDialog;

        // Maximal vier Figuren auswählbar
        private const int MaxSelections = 4;
        private int _countSelectedCharacters;

        // Verweise auf Bestätigen und Abbrechen-Buttons
        [Export] private TextureButton _confirmSelectionButton;
        [Export] private TextureButton _returnSelectionButton;

        //Zustandsvariablen der Charakterbuttons
        private bool _isAmaraPressed;
        private bool _isAthenaPressed;
        private bool _isAxtonPressed;
        private bool _isGaigePressed;
        private bool _isKriegPressed;
        private bool _isMajaPressed;
        private bool _isMozePressed;
        private bool _isNishaPressed;
        private bool _isSalvadorPressed;
        private bool _isWhilhelmPressed;
        private bool _isZeroPressed;

        /// <summary>
        /// Liste der Spielfiguren von User
        /// </summary>
        // List to keep track of selected characters
        public List<string> CharactersPlayer = new();


        /// <summary>
        /// Initialize all label nodes with the text color "white",
        /// deactivate the "confirmSelectionButton" button and make it invisible
        /// </summary>
        public override void _Ready()
        {
            // Setze den Anfangszustand für die Buttons
            _confirmSelectionButton.Visible = false;
            _confirmSelectionButton.Disabled = true;
            _returnSelectionButton.Disabled = false;
            _headZeroButton.Disabled = false;
            _headWhilhelmButton.Disabled = false;
            _headSalvadorButton.Disabled = false;
            _headNishaButton.Disabled = false;
            _headMozeButton.Disabled = false;
            _headMajaButton.Disabled = false;
            _headKriegButton.Disabled = false;
            _headGaigeButton.Disabled = false;
            _headAxtonButton.Disabled = false;
            _headAthenaButton.Disabled = false;
            _headAmaraButton.Disabled = false;

            //Weieße Lable-Farbe bei Szenenaufruf setzten
            _zeroLabel.Modulate = new Color(255, 255, 255);
            _whilhelmLabel.Modulate = new Color(255, 255, 255);
            _salvadorLabel.Modulate = new Color(255, 255, 255);
            _nishaLabel.Modulate = new Color(255, 255, 255);
            _mozeLabel.Modulate = new Color(255, 255, 255);
            _majaLabel.Modulate = new Color(255, 255, 255);
            _kriegLabel.Modulate = new Color(255, 255, 255);
            _gaigeLabel.Modulate = new Color(255, 255, 255);
            _axtonLabel.Modulate = new Color(255, 255, 255);
            _athenaLabel.Modulate = new Color(255, 255, 255);
            _amaraLabel.Modulate = new Color(255, 255, 255);

            //Überprüfe funktionlität ConfirmSelectionButton
            CheckSelectedCharacters();
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnAthenaHeadPressed()
        {
            if (!_isAthenaPressed)
            {
                _isAthenaPressed = true;
                _athenaLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Athena");
                CheckSelectedCharacters();
            }
            else
            {
                _isAthenaPressed = false;
                _athenaLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Athena");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnAmaraHeadPressed()
        {
            if (!_isAmaraPressed)
            {
                _isAmaraPressed = true;
                _amaraLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Amara");
                CheckSelectedCharacters();
            }
            else
            {
                _isAmaraPressed = false;
                _amaraLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Amara");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnAxtonHeadPressed()
        {
            if (!_isAxtonPressed)
            {
                _isAxtonPressed = true;
                _axtonLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Axtona");
                CheckSelectedCharacters();
            }
            else
            {
                _isAxtonPressed = false;
                _axtonLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Axton");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnGaigeHeadPressed()
        {
            if (!_isGaigePressed)
            {
                _isGaigePressed = true;
                _gaigeLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Gaige");
                CheckSelectedCharacters();
            }
            else
            {
                _isGaigePressed = false;
                _gaigeLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Gaige");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnKriegHeadPressed()
        {
            if (!_isKriegPressed)
            {
                _isKriegPressed = true;
                _kriegLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Krieg");
                CheckSelectedCharacters();
            }
            else
            {
                _isKriegPressed = false;
                _kriegLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Krieg");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnMajaHeadPressed()
        {
            if (!_isMajaPressed)
            {
                _isMajaPressed = true;
                _majaLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Maja");
                CheckSelectedCharacters();
            }
            else
            {
                _isMajaPressed = false;
                _majaLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Maja");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnMozeHeadPressed()
        {
            if (!_isMozePressed)
            {
                _isMozePressed = true;
                _mozeLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Moze");
                CheckSelectedCharacters();
            }
            else
            {
                _isMozePressed = false;
                _mozeLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Moze");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnNishaHeadPressed()
        {
            if (!_isNishaPressed)
            {
                _isNishaPressed = true;
                _nishaLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Nisha");
                CheckSelectedCharacters();
            }
            else
            {
                _isNishaPressed = false;
                _nishaLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Nisha");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnSalvadorHeadPressed()
        {
            if (!_isSalvadorPressed)
            {
                _isSalvadorPressed = true;
                _salvadorLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Salvador");
                CheckSelectedCharacters();
            }
            else
            {
                _isSalvadorPressed = false;
                _salvadorLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Salvador");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnWilhelmHeadPressed()
        {
            if (!_isWhilhelmPressed)
            {
                _isWhilhelmPressed = true;
                _whilhelmLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Whilhelm");
                CheckSelectedCharacters();
            }
            else
            {
                _isWhilhelmPressed = false;
                _whilhelmLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Whilhelm");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Auswahl Spielfigur, erhöhe Anzahl um 1, verändere Labelcolor
        /// </summary>
        private void OnZeroHeadPressed()
        {
            if (!_isZeroPressed)
            {
                _isZeroPressed = true;
                _zeroLabel.Modulate = new Color(0, 255, 0);
                _countSelectedCharacters++;
                CharactersPlayer.Add("Zero");
                CheckSelectedCharacters();
            }
            else
            {
                _isZeroPressed = false;
                _zeroLabel.Modulate = new Color(255, 255, 255);
                _countSelectedCharacters--;
                _ = CharactersPlayer.Remove("Zero");
                CheckSelectedCharacters();
            }
        }

        /// <summary>
        /// Üperprüfe Anzahl ausgewählter Spielfiguren, aktiviere ConfirmSelectionButton
        /// </summary>
        private void CheckSelectedCharacters()
        {
            if (_countSelectedCharacters == MaxSelections)
            {
                _confirmSelectionButton.Visible = true;
                _confirmSelectionButton.Disabled = false;
            }
            else
            {
                _confirmSelectionButton.Visible = false;
                _confirmSelectionButton.Disabled = true;
            }
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
                _exitDialog = new GameConfirmationDialog("Charakterauswahl verlassen",
                    "Möchtest du die Charakterauswahl abbrechen?");
                _exitDialog.GetOkButton().Pressed += () =>
                    _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/MainMenu.tscn");
                AddChild(_exitDialog);
            }

            _exitDialog.Visible = true;
        }
    };
}