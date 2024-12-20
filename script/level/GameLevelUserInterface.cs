using Godot;

using INTOnlineCoop.Script.Item;
using INTOnlineCoop.Script.Player;
using INTOnlineCoop.Script.Singleton;

namespace INTOnlineCoop.Script.Level
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GameLevelUserInterface : CanvasLayer
    {
        /// <summary>
        /// Color of the first player
        /// </summary>
        public static readonly Color PlayerOneColor = Color.Color8(255, 70, 54);

        /// <summary>
        /// Color of the second player
        /// </summary>
        public static readonly Color PlayerTwoColor = Color.Color8(97, 146, 255);

        // Timer Nodes exportieren
        [Export] private Timer _timer;
        [Export] private Label _labelTime;

        // PlayerCharacter Listennodes
        [Export] private Control _barPlayer1;
        [Export] private Control _barPlayer2;
        [Export] private RichTextLabel _labelPlayer1;
        [Export] private RichTextLabel _labelPlayer2;
        [Export] private Sprite2D[] _spritesPlayer1;
        [Export] private Sprite2D[] _spritesPlayer2;

        [Export] private Label _playerNotificationLabel;
        [Export] private Label _waterNotificationLabel;
        [Export] private Node _characterParent;

        // Waffenbutton Node Liste
        [Export] private HBoxContainer _weaponContainer;
        [Export] private TextureButton _textureButtonBazooka;
        [Export] private TextureButton _textureButtonPistol;
        [Export] private TextureButton _textureButtonShotgun;
        [Export] private TextureButton _textureButtonSniper;

        private bool _areSignalsConnected;

        //Hilfsvariable für Todeszustände der SPielfiguren
        //private readonly bool[] _player1CharacterDead;
        //private readonly bool[] _player2CharacterDead;

        //Hilfsvariable Anzahl an möglichen Schüssen von Waffen
        //(x = -1: undendlich, x = 0: alle Schüsse aufgebraucht, x>0: Waffe abschießbereit)
        private int _bazzokaUsageNumber = -1;
        private int _pistolUsageNumber;
        private int _shotgunUsageNumber;
        private int _sniperUsageNumber;

        // Hilfvariable um zu erkennen, ob geschossen wurde oder nicht, nachdem Waffe ausgewählt wurde
        private bool _shotTaken;

        /// <summary>
        /// Zuweisen der privaten Variablen auf alle genutzen UI Nodes
        /// und Farbe initialisieren bei Timer Label
        /// </summary>
        public override void _Ready()
        {
            // Setze Default Farbe für Timer Label
            _labelTime.AddThemeColorOverride("font_color", Color.Color8(255, 255, 255));

            //Spielfigurenzustände zu Beginn auf "lebend" setzen
            // _isFigureA1Dead = false 

            foreach (PlayerData data in MultiplayerLobby.Instance.GetPlayerData())
            {
                int playerNumber = data.PlayerNumber;
                UpdateCharacterIcons(playerNumber, data.Characters);
                SetPlayerName(playerNumber, data.Name);
            }

            //aktivieren aller Waffen Button
            //Bazzoka = unendliche Schüsse, Sniper und Shotgun auf x zum Start festlegen,
            //Pistole wegen möglichen Singel oder Tripel-Schuss unterscheiden von Anzhal Schüssen
        }

        /// <summary>
        /// Update timer label
        /// </summary>
        /// <param name="delta">Current frame delta</param>
        public override void _Process(double delta)
        {
            // Zeit-Label aktualisieren
            UpdateTimeLabel();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateTimeLabel()
        {
            // Verbleibende Zeit vom Timer abrufen
            float timeLeft = (float)_timer.TimeLeft;
            if (timeLeft >= 60 && !_labelTime.Visible)
            {
                _labelTime.Visible = true;
            }

            //Verändere Color vom TimeLable, wenn timeLeft unter 10 s
            _labelTime.AddThemeColorOverride("font_color",
                timeLeft <= 10f ? Color.Color8(255, 0, 0) : Color.Color8(255, 255, 255));

            // Zeit formatieren (Minuten:Sekunden) und auf Label anzeigen
            _labelTime.Text = FormatTime(timeLeft);
        }

        [Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void HideTimerLabel()
        {
            _labelTime.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static string FormatTime(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            return $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Displays the turn notification
        /// </summary>
        /// <param name="playerName">Name of the player</param>
        /// <param name="playerNumber">Player number</param>
        /// <param name="isWaterRising">True if the water is rising</param>
        public void DisplayTurnNotification(string playerName, int playerNumber, bool isWaterRising)
        {
            if (_playerNotificationLabel == null || _waterNotificationLabel == null)
            {
                return;
            }

            if (!_areSignalsConnected)
            {
                ConnectPlayerSignals();
                _areSignalsConnected = true;
            }

            _playerNotificationLabel.Visible = true;
            _playerNotificationLabel.Text = playerName + " ist am Zug!";
            Color textColor = playerNumber == 1 ? PlayerOneColor : PlayerTwoColor;
            _playerNotificationLabel.AddThemeColorOverride("font_color", textColor);
            if (isWaterRising)
            {
                _waterNotificationLabel.Visible = true;
                _waterNotificationLabel.AddThemeColorOverride("font_color", textColor);
            }

            GetTree().CreateTimer(5).Timeout += () =>
            {
                _playerNotificationLabel.Visible = false;
                _waterNotificationLabel.Visible = false;
            };
        }

        private void ConnectPlayerSignals()
        {
            if (_characterParent == null)
            {
                return;
            }

            foreach (Node node in _characterParent.GetChildren())
            {
                if (node is not PlayerCharacter character)
                {
                    continue;
                }

                character.PlayerDied += KillCharacterIcon;
            }
        }

        /// <summary>
        /// Disconnects player signals
        /// </summary>
        public override void _ExitTree()
        {
            if (!_areSignalsConnected)
            {
                return;
            }

            foreach (Node node in _characterParent.GetChildren())
            {
                if (node is not PlayerCharacter character)
                {
                    continue;
                }

                character.PlayerDied -= KillCharacterIcon;
            }
        }

        /// <summary>
        /// Updates the display status of the weapons
        /// </summary>
        /// <param name="playerNumber">Number of the player who is on turn</param>
        public void DisplayWeapons(int playerNumber)
        {
            long peerId = Multiplayer.GetUniqueId();
            if (peerId == 1 || _weaponContainer == null)
            {
                return;
            }

            _weaponContainer.Visible = MultiplayerLobby.Instance.GetPlayerData(peerId).PlayerNumber == playerNumber;
        }

        /// <summary>
        /// Changes the character icons of a player
        /// </summary>
        /// <param name="playerNumber">Number of the player</param>
        /// <param name="characters">Current characters</param>
        public void UpdateCharacterIcons(int playerNumber, CharacterType[] characters)
        {
            Sprite2D[] playerSprites = playerNumber == 1 ? _spritesPlayer1 : _spritesPlayer2;
            if (characters.Length != 4 || playerSprites.Length != 4)
            {
                GD.PrintErr("Wrong number of characters in GameLevel UI");
                return;
            }

            for (int i = 0; i < playerSprites.Length; i++)
            {
                playerSprites[i].Texture = characters[i].HeadTexture;
            }
        }

        /// <summary>
        /// Updates the icon of a killed character
        /// </summary>
        /// <param name="character">Died character</param>
        public void KillCharacterIcon(PlayerCharacter character)
        {
            PlayerData data = MultiplayerLobby.Instance.GetPlayerData(character.PeerId);
            int playerNumber = data.PlayerNumber;
            Sprite2D[] playerSprites = playerNumber == 1 ? _spritesPlayer1 : _spritesPlayer2;

            for (int i = 0; i < playerSprites.Length; i++)
            {
                CharacterType type = data.Characters[i];
                if (type == character.Type)
                {
                    playerSprites[i].Texture = GD.Load<Texture2D>("res://assets/texture/PlayerDead.png");
                    break;
                }
            }

            // Hide player information
            character.HideHealth();
            character.HideCharacterIcon();
            character.StateMachine.TransitionTo(AvailableState.Dead);
        }

        /// <summary>
        /// Sets the name of a player
        /// </summary>
        /// <param name="playerNumber">Number of the player</param>
        /// <param name="playerName">Username</param>
        public void SetPlayerName(int playerNumber, string playerName)
        {
            RichTextLabel playerLabel = playerNumber == 1 ? _labelPlayer1 : _labelPlayer2;
            if (playerLabel == null)
            {
                return;
            }

            long currentPeer = Multiplayer.GetUniqueId();
            if (currentPeer == 1)
            {
                return;
            }

            bool isCurrentClient = MultiplayerLobby.Instance.GetPlayerData(currentPeer).PlayerNumber == playerNumber;
            string prefix = isCurrentClient ? "[right][u]" : "[right]";
            playerLabel.Text = prefix + playerName;
        }

        /// <summary>
        /// Hides the player bars
        /// </summary>
        public void HidePlayerBars()
        {
            _barPlayer1.Visible = false;
            _barPlayer2.Visible = false;
        }

        private void OnBazookaButtonPressed()
        {
            UpdateWeapon(SelectableItem.Bazooka);
        }

        private void OnPistolButtonPressed()
        {
            UpdateWeapon(SelectableItem.Pistol);
        }

        private void OnShotgunButtonPressed()
        {
            UpdateWeapon(SelectableItem.Shotgun);
        }

        private void OnSniperButtonPressed()
        {
            UpdateWeapon(SelectableItem.Sniper);
        }

        private void UpdateWeapon(SelectableItem item)
        {
            Error error = GetCurrentCharacter().Rpc(PlayerCharacter.MethodName.SetItem, item.Name);
            if (error != Error.Ok)
            {
                GD.PrintErr("Error during SetWeapon RPC: " + error);
            }
        }

        private PlayerCharacter GetCurrentCharacter()
        {
            foreach (Node node in _characterParent.GetChildren())
            {
                if (node is not PlayerCharacter character)
                {
                    continue;
                }

                if (!character.IsBlocked)
                {
                    return character;
                }
            }

            return null;
        }
    }
}