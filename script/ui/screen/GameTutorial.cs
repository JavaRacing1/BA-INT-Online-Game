using Godot;

using System.Collections.Generic;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// Darstellung verschiedener Spielbeschreibungstexte und -bilder.
    /// Für jede Szene, die beschrieben werden soll, neuen Texteintrag in Array anlegen.
    /// Bilder in das Verzeichnis assets/texture/tutorial/ mit den Namen "Page(i)" anlegen (also Page1, Page2, ...).
    /// Bilderanzahl und StringArray Abschnitte müssen gleich sein, sonst wird Fehlermeldung in COnsole erzeugt.
    /// Bitte Text nach Länge und Fontgröße den Bilder anpassen!
    /// </summary>
    public partial class GameTutorial : Control
    {
        private static readonly Dictionary<int, Texture2D> TextureCache = new();
        private readonly string[] _tutorialText =
            {"Willkommen beim BA INT-Game!\nDiese kleine Einführung soll dem Spieler die grundlegenden Prinzipien, Inhalte und Ziele des Spiels vermitteln.",
         "",
         "",
         "",
         "",
         "",
         "",
         "" ,
         ""};

        private int _tutorialTextIndex;
        private Label _infoText, _pageLabel;
        private TextureRect _imagePanel;
        private Button _showNextButton, _showPreviousButton;

        /// <summary>
        /// Initialisierung Bilder und Textbox mit ersten Eintrag
        /// Traversieren TextArray und BilderOrdner über Schleife
        /// </summary>
        public override void _Ready()
        {
            _infoText = GetNode<Label>("InfoTextLabel");
            _imagePanel = GetNode<TextureRect>("InfoImage");
            _showPreviousButton = GetNode<Button>("ShowPreviousButton");
            _showNextButton = GetNode<Button>("ShowNextButton");
            _pageLabel = GetNode<Label>("PageLabel");

            //vorab laden der Seitenhintergründe, um ein langsames Laden beim Seitenwechsel zu vermeiden
            for (int i = 0; i < _tutorialText.Length; i++)
            {
                string texturePath = $"res://assets/texture/tutorial/Page{i}.png";
                if (ResourceLoader.Exists(texturePath))
                {
                    if (!TextureCache.ContainsKey(i))
                    {
                        Texture2D texture = GD.Load<Texture2D>(texturePath);
                        TextureCache.Add(i, texture);
                    }
                }
                else
                {
                    GD.PrintErr($"Tutorial-Bild für Seite {i + 1} konnte nicht gefunden werden!");
                }
            }

            UpdateUserInterface();
        }

        /// <summary>
        /// Wechsel zurück in das Hauptmenü
        /// </summary>
        private void OnMainMenuButtonPressed()
        {
            _ = GetTree().ChangeSceneToFile("res://scene/ui/MainMenu.tscn");
        }
        /// <summary>
        /// Anzeigen der nächsten Seite und Beschreibung
        /// </summary>
        private void OnShowNextButtonPressed()
        {
            if (_tutorialTextIndex < _tutorialText.Length)
            {
                _tutorialTextIndex++;
                UpdateUserInterface();
            }
        }

        /// <summary>
        /// Anzeigen der vorherigen Seite und Beschreibung
        /// </summary>
        private void OnShowPreviousButtonPressed()
        {
            if (_tutorialTextIndex > 0)
            {
                _tutorialTextIndex--;
                UpdateUserInterface();
            }
        }
        /// <summary>
        /// Wechsel der verschiedenen Bilder und zugehörigen Textabschnitte aus dem Array
        /// </summary>
        private void UpdateUserInterface()
        {
            _showPreviousButton.Disabled = _tutorialTextIndex == 0;
            _showNextButton.Disabled = _tutorialTextIndex == (_tutorialText.Length - 1);

            _infoText.Text = _tutorialText[_tutorialTextIndex];
            _pageLabel.Text = $"{_tutorialTextIndex + 1}/{_tutorialText.Length}";

            if (TextureCache.ContainsKey(_tutorialTextIndex))
            {
                _imagePanel.Texture = TextureCache[_tutorialTextIndex];
            }
            else
            {
                _imagePanel.Texture = null;
            }
        }
    }
}

