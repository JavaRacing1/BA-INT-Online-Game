using Godot;

using System.Linq;

using System.Collections.Generic;
public partial class StateMachine : Node
{
    //eigene Signale der StateMachine, wenn immer ein neuer State in der StateMachine gestartet wird
    [Signal] public delegate void PreStarEventHandler();

    [Signal] public delegate void PostStartEventHandler();

    [Signal]
    public delegate void PreExitEventHandler();

    [Signal] public delegate void PostExitEventHandler();

    //Liste aller States
    public List<SimpleState> States;

    //aktueller State, der in Betrieb ist
    public string CurrentState;

    //zuletzt bearbeiteter State
    public string LastState;

    //Status für StateMachine
    private SimpleState _state;

    public override void _Ready()
    {
        base._Ready();
        // hol die State-Nodes ("Walk"/"Air"/"Idle"/"Inaktiv") aus einer Szene mit Parent-Node "States" und die vom Typ "SimpleStates" sind
        // Ziel:
        States = GetNode<Node>("States").GetChildren().OfType<SimpleState>().ToList();
    }

    //SetState benötigt State von StateMachine und message 
    private void SetState(SimpleState state, Dictionary<string, object> message)
    {
        if (state == null) //Überprüfe, ob Statemachine State inaktiv
        {
            return;
        }
        if (_state != null) //Überprüfe, ob derzeitiger State des Systems ungleich null
        {
            EmitSignal(nameof(PreExit)); //Signal Begin den aktuellen State zu verlassen
            state.OnExit(state.GetType().ToString()); //Verlassenfunktion
            EmitSignal(nameof(PostExit)); //Signal Erfolgreiches Verlassen des aktuellen States
        }

        LastState = CurrentState;
        CurrentState = state.GetType().ToString(); //nimmt den nächsten State aus Liste/Animationstree/ etc. an

        _state = state;
        EmitSignal(nameof(PreStar)); //Signal zu Einsetzen des Statewechsels
        state.OnStart(message); //Funktion des Statewechsels mit Information (wie Lebenspunkte, Spielfigurenanzahl, etc.) als message
        EmitSignal(nameof(PostStart)); //Signal Erfolgreiches Wechseln State
    }

    //Anforderung eines Statewechsels von außen
    public void ChangeState(string stateName, Dictionary<string, object> message = null)
    {
        foreach (SimpleState state in States) //geh alle States durch die in der Liste existieren
        {
            if (stateName == state.GetType().ToString()) //wenn der aktuelle State, zu den gewechselt werden soll, gleich dem in der Liste ist
            {
                SetState(state, message); //Wechsel die States
                return;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_state == null)
        {
            return;
        }

        _state.UpdateState(delta);
    }
}
