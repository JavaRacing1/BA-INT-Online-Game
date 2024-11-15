using Godot;

namespace INTOnlineCoop.Script.Player.States
{
    /// <summary>
    /// State used when the character is walking
    /// </summary>
    public partial class Walking : State
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

            Character.Velocity = velocity;
            _ = Character.MoveAndSlide();

            if (!Character.IsOnFloor())
            {
                Character.StateMachine.TransitionTo(AvailableState.Falling);
            }
            else if (Input.IsActionJustPressed("jump"))
            {
                Character.StateMachine.TransitionTo(AvailableState.Jumping);
            }
            else if (Mathf.IsEqualApprox(inputDirection, 0.0))
            {
                Character.StateMachine.TransitionTo(AvailableState.Idle);
            }
        }
    }
}