using Godot;

namespace INTOnlineCoop.Script.Player.States
{
    /// <summary>
    /// State used when player is falling
    /// </summary>
    public partial class Falling : State
    {
        /// <summary>
        /// Updates player movement
        /// </summary>
        /// <param name="delta">Current frame delta</param>
        public override void PhysicProcess(double delta)
        {
            Vector2 velocity = Character.Velocity;

            float inputDirection = Input.GetAxis("walk_left", "walk_right");
            velocity.X = inputDirection * Speed;
            velocity.Y += Gravity * (float)delta;

            Character.Velocity = velocity;
            _ = Character.MoveAndSlide();

            if (Character.IsOnFloor())
            {
                Character.StateMachine.TransitionTo(Mathf.IsEqualApprox(inputDirection, 0.0)
                    ? AvailableState.Idle
                    : AvailableState.Walking);
            }
        }
    }
}