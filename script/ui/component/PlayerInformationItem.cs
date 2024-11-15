using System.Collections.Generic;

using Godot;

namespace INTOnlineCoop.Script.UI.Component
{
    /// <summary>
    /// Displays information about a player
    /// </summary>
    public partial class PlayerInformationItem : PanelContainer
    {
        [Export] private Label _numberLabel;
        [Export] private Label _nameLabel;

        [Export] private List<Sprite2D> Figures = new List<Sprite2D>();
        /// <summary>
        /// Changes the player number of the instance
        /// </summary>
        /// <param name="number">New player number</param>
        public void SetPlayerNumber(int number)
        {
            if (_numberLabel != null)
            {
                _numberLabel.Text = number.ToString();
            }
        }

        /// <summary>
        /// Changes the player name of the instance
        /// </summary>
        /// <param name="name">New player name</param>
        public void SetPlayerName(string name)
        {
            if (_nameLabel != null)
            {
                _nameLabel.Text = name;
            }
        }

        /// <summary>
        /// Returns the player number
        /// </summary>
        /// <returns>Current player number</returns>
        public int GetPlayerNumber()
        {
            return _numberLabel == null ? -1 : int.Parse(_numberLabel.Text);
        }
    }
}
