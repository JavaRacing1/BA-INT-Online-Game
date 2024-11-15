using System;

using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script.Item
{
    /// <summary>
    /// Item for which the exact target position needs to be specified
    /// </summary>
    public abstract partial class PositionItem : Node, IItem
    {
        private const int AimSpeed = 4;
        private Vector2 _initialPosition = Vector2.Zero;

        /// <summary>
        /// Handles the input for the position item. Should be called every frame
        /// </summary>
        /// <param name="characterPosition">Current position of the character</param>
        /// <param name="direction">Direction in which the character looks</param>
        public void HandleInput(Vector2 characterPosition, CharacterFacingDirection direction)
        {
            if (GameLevel.IsInputBlocked)
            {
                return;
            }

            if (_initialPosition == Vector2.Zero)
            {
                _initialPosition = characterPosition;
            }

            if (Input.IsActionJustPressed("use_item"))
            {
                UseItem(_initialPosition);
                return;
            }

            Vector2 movement = Vector2.Zero;
            if (Input.IsActionPressed("walk_left"))
            {
                movement += Vector2.Left;
            }

            if (Input.IsActionPressed("walk_right"))
            {
                movement += Vector2.Right;
            }

            if (Input.IsActionPressed("aim_up"))
            {
                movement += Vector2.Up;
            }

            if (Input.IsActionPressed("aim_down"))
            {
                movement += Vector2.Down;
            }

            _initialPosition += movement * AimSpeed;

            Camera2D activeCamera = GetViewport().GetCamera2D();
            if (activeCamera != null)
            {
                if (Input.IsMouseButtonPressed(MouseButton.Left))
                {
                    _initialPosition = activeCamera.GetGlobalMousePosition();
                }

                float limitedX = Math.Clamp(_initialPosition.X, activeCamera.LimitLeft, activeCamera.LimitRight);
                float limitedY = Math.Clamp(_initialPosition.Y, activeCamera.LimitTop, activeCamera.LimitBottom);
                _initialPosition = new Vector2(limitedX, limitedY);
            }
        }

        /// <summary>
        /// Activates the item at the given position
        /// </summary>
        /// <param name="targetPosition">Target position for usage</param>
        protected abstract void UseItem(Vector2 targetPosition);
    }
}