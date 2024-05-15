using System;

using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script.Item
{
    /// <summary>
    /// Item with aiming -> Only direction is required
    /// </summary>
    public abstract partial class AimingItem : Node, IItem
    {
        private int _rotation;
        private CharacterFacingDirection _lastFacingDirection;

        /// <summary>
        /// Handles the input for the aiming item. Should be called every frame
        /// </summary>
        /// <param name="characterPosition">Current position of the character</param>
        /// <param name="direction">Direction in which the character looks</param>
        public void HandleInput(Vector2 characterPosition, CharacterFacingDirection direction)
        {
            if (GameLevel.IsInputBlocked)
            {
                return;
            }

            if (Input.IsActionJustPressed("use_item"))
            {
                UseItem(characterPosition, _rotation);
            }

            if (direction != _lastFacingDirection)
            {
                _rotation += direction == CharacterFacingDirection.Left ? 180 : -180;
                _lastFacingDirection = direction;
            }

            int rotationModifier = 0;
            if (Input.IsActionPressed("aim_up"))
            {
                rotationModifier += direction == CharacterFacingDirection.Left ? 1 : -1;
            }
            if (Input.IsActionPressed("aim_down"))
            {
                rotationModifier += direction == CharacterFacingDirection.Left ? -1 : 1;
            }

            _rotation = direction == CharacterFacingDirection.Left
                ? Math.Clamp(_rotation + rotationModifier, 180, 360)
                : Math.Clamp(_rotation + rotationModifier, 0, 180);
        }

        /// <summary>
        /// Activates the item at the given position with the given rotation
        /// </summary>
        /// <param name="targetPosition">Target position for usage</param>
        /// <param name="rotation">Rotation of the aiming (up == 0, right == 90, etc.)</param>
        protected abstract void UseItem(Vector2 targetPosition, int rotation);
    }
}