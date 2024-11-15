using Godot;

namespace INTOnlineCoop.Script.Player
{
    /// <summary>
    /// Character controlled by the player
    /// </summary>
    public partial class PlayerCharacter : CharacterBody2D
    {
        /// <summary>
        /// Current StateMachine instance
        /// </summary>
        [Export]
        public StateMachine StateMachine { get; private set; }

        /// <summary>
        /// Initializes the state machine
        /// </summary>
        public override void _Ready()
        {
            if (StateMachine == null)
            {
                return;
            }

            StateMachine.TransitionTo(AvailableState.Idle); // Initialer Zustand
        }

        /// <summary>
        /// Redirects physic and movement updates to states
        /// </summary>
        /// <param name="delta">Current Frame-delta</param>
        public override void _PhysicsProcess(double delta)
        {
            StateMachine.CurrentState.PhysicProcess(delta);
        }

        /// <summary>
        /// Redirects input to states
        /// </summary>
        /// <param name="event">InputEvent instance</param>
        public override void _UnhandledInput(InputEvent @event)
        {
            StateMachine.CurrentState.HandleInput(@event);
        }
    }
}