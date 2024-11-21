using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script.Player.States
{
    /// <summary>
    /// State used when the character is walking
    /// </summary>
    public partial class Walking : State
    {
        /// <summary>
        /// Handles walking input
        /// </summary>
        /// <param name="delta">Current frame delta</param>
        public override void HandleInput(double delta)
        {
            if (GameLevel.IsInputBlocked || Character.IsBlocked || Character.PeerId != Multiplayer.GetUniqueId())
            {
                return;
            }

            StateMachine.Direction = Input.GetAxis("walk_left", "walk_right");

            if (Input.IsActionJustPressed("jump"))
            {
                Error error = StateMachine.Rpc(StateMachine.MethodName.Jump);
                if (error != Error.Ok)
                {
                    GD.PrintErr($"Error during Jump RPC: {error}");
                }
            }
        }

        /// <summary>
        /// Changes animations and states
        /// </summary>
        /// <param name="delta"></param>
        public override void ChangeAnimationsAndStates(double delta)
        {
            if (!Mathf.IsEqualApprox(StateMachine.Direction, 0))
            {
                CharacterSprite.FlipH = StateMachine.Direction < 0;
            }

            if (!Character.IsOnFloor())
            {
                CharacterSprite.Stop();
                CharacterSprite.Animation = "InAir";
                CharacterSprite.Frame = 1;
                CharacterSprite.Play("InAir");
                Character.StateMachine.TransitionTo(AvailableState.Falling);
            }
            else if (StateMachine.Jumped)
            {
                CharacterSprite.Stop();
                CharacterSprite.Play("JumpingOffGround");
                Character.StateMachine.TransitionTo(AvailableState.Jumping);
            }
            else if (Mathf.IsEqualApprox(StateMachine.Direction, 0))
            {
                CharacterSprite.Stop();
                Character.StateMachine.TransitionTo(AvailableState.Idle);
            }
        }

        /// <summary>
        /// Updates player movement
        /// </summary>
        /// <param name="delta">Current frame delta</param>
        public override void PhysicProcess(double delta)
        {
            Vector2 velocity = Character.Velocity;
            velocity.X = StateMachine.Direction * Speed;
            Character.Velocity = velocity;
            _ = Character.MoveAndSlide();
        }
    }
}