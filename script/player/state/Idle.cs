using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script.Player.States
{
    /// <summary>
    /// State used when the character is not moving
    /// </summary>
    public partial class Idle : State
    {
        // private bool _gettingHit;
        private int _idleFrameCounter;

        /// <summary>
        /// define what animation is played when entering the state
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            if (!Character.TexturesLoaded)
            {
                Character.LoadTextures();
            }
            CharacterSprite.Animation = "Idle";
            CharacterSprite.Pause();
            StateMachine.Jumped = false;
            StateMachine.HasDoubleJumped = false;
            StateMachine.Direction = 0;
            _idleFrameCounter = 0;
        }

        /// <summary>
        /// Handles input in Idle state
        /// </summary>
        /// <param name="delta">Current frame delta</param>
        public override void HandleInput(double delta)
        {
            if (!Multiplayer.HasMultiplayerPeer() || Character.PeerId != Multiplayer.GetUniqueId())
            {
                return;
            }

            if (GameLevel.IsInputBlocked || Character.IsBlocked)
            {
                StateMachine.Direction = 0;
                return;
            }

            Character.CurrentItem?.HandleInput(delta);

            if (Input.IsActionJustPressed("jump"))
            {
                Error error = StateMachine.Rpc(StateMachine.MethodName.Jump);
                if (error != Error.Ok)
                {
                    GD.PrintErr($"Error during Jump RPC: {error}");
                }
            }

            StateMachine.Direction = Input.GetAxis("walk_left", "walk_right");
        }

        /// <summary>
        /// Manages the used animations + state changes
        /// </summary>
        /// <param name="delta">Frame delta</param>
        public override void ChangeAnimationsAndStates(double delta)
        {
            if (!Character.HasWeapon)
            {
                _idleFrameCounter++;
                if (_idleFrameCounter == 0)
                {
                    CharacterSprite.Frame = 0;
                }
                else if (CharacterSprite.Frame == Character.Type.LastIdleFrame && _idleFrameCounter > 0)
                {
                    CharacterSprite.Pause();
                    _idleFrameCounter = -20;
                }
                else if (_idleFrameCounter == 400)
                {
                    CharacterSprite.Play();
                }
            }
            else
            {
                _idleFrameCounter = 0;
                CharacterSprite.Pause();
                CharacterSprite.Frame = 0;

                if (!Mathf.IsEqualApprox(StateMachine.ItemRotation, 0))
                {
                    float oldRotation = Character.CurrentItem.RotationDegrees;
                    Character.CurrentItem.RotationDegrees = Mathf.Clamp(oldRotation + StateMachine.ItemRotation, -70, 70);
                }
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
            else if (!Mathf.IsEqualApprox(StateMachine.Direction, 0.0))
            {
                CharacterSprite.Stop();
                CharacterSprite.FlipH = StateMachine.Direction < 0;
                Character.UpdateWeaponDirection();
                CharacterSprite.Play("Walking");
                Character.StateMachine.TransitionTo(AvailableState.Walking);
            }
        }
    }
}