using Godot;

namespace INTOnlineCoop.Script.Item
{
    /// <summary>
    /// Direction in which the character looks
    /// </summary>
    public enum CharacterFacingDirection
    {
        /// <summary> Character looks to the left </summary>
        Left,
        /// <summary> Character looks to the right </summary>
        Right
    }
    /// <summary>
    /// Interface for a character item
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Handles the input for the item
        /// </summary>
        /// <param name="characterPosition">Current position of the character</param>
        /// <param name="direction">Direction in which the character looks</param>
        void HandleInput(Vector2 characterPosition, CharacterFacingDirection direction);
    }
}