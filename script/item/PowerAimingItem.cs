using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script.Item
{
    /// <summary>
    /// Item with aiming -> Direction and power is required
    /// </summary>
    public abstract class PowerAimingItem : IItem
    {
        private int _rotation;
        private int _power;

        /// <summary>
        /// Handles the input for the aiming item
        /// </summary>
        /// <param name="inputEvent">Input event</param>
        /// <param name="playerPosition">Current position of the player</param>
        public void HandleInput(InputEvent inputEvent, Vector2 playerPosition)
        {
            if (GameLevel.IsInputBlocked)
            {
                return;
            }

            if (inputEvent.IsAction("use_item"))
            {
                UseItem(playerPosition, _rotation, _power);
            }

            //TODO: Add rotation + power change
            _rotation = _rotation;
            _power = _power;
        }

        /// <summary>
        /// Activates the item at the given position with the given rotation and power
        /// </summary>
        /// <param name="targetPosition">Target position for usage</param>
        /// <param name="rotation">Rotation of the aiming (up == 0, right == 90, etc.)</param>
        /// <param name="power">Strength of the shot (maximum power = 100)</param>
        protected abstract void UseItem(Vector2 targetPosition, int rotation, int power);
    }
}