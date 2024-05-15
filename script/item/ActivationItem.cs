using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script.Item
{
    /// <summary>
    /// Item without any aiming
    /// </summary>
    public abstract partial class ActivationItem : Node, IItem
    {
        /// <summary>
        /// Handles the input for the activation item. Should be called every frame
        /// </summary>
        /// <param name="characterPosition">Current position of the character</param>
        /// <param name="direction">Direction in which the character looks</param>
        public void HandleInput(Vector2 characterPosition, CharacterFacingDirection direction)
        {
            if (!GameLevel.IsInputBlocked && Input.IsActionJustPressed("use_item"))
            {
                UseItem(characterPosition);
            }
        }

        /// <summary>
        /// Activates the item at the given position
        /// </summary>
        /// <param name="targetPosition">Target position for usage</param>
        protected abstract void UseItem(Vector2 targetPosition);
    }
}