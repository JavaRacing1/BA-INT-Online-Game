using Godot;

using System.Linq;

using System.Collections.Generic;
public partial class StateMachine : Node
{
    //eigene Signale der StateMachine, wenn immer ein neuer State in der StateMachine gestartet wird
    [Signal] public delegate void PreStar();

    [Signal] public delegate void PostStart();

    [Signal] public delegate void PreExit():

    [Signal] public delegate void PostExit();

    //Liste aller States
    public List<SimpleState> States;

    //aktueller State, der in Betrieb ist
    public string CurrentState;

    //zuletzt bearbeiteter State
    public string LastState;

    //Status für StateMachine
    protected SimpleState state = null;

    public override void _Ready()
    {
        base._Ready();
        // hol die State-Nodes ("Walk"/"Air"/"Idle"/"Inaktiv") aus einer Szene mit Parent-Node "States" und die vom Typ "SimpleStates" sind
        // Ziel:
        States = GetNode<Node>("States").GetChildren().OfType<SimpleState>().ToList();
    }

    //SetState benötigt State von StateMachine und message 
    private void SetState(SimpleState _state, Dictionary<string, object> message)
    {
        if (_state == null) //Überprüfe, ob Statemachine State inaktiv
        {
            return;
        }
        if (state != null) //Überprüfe, ob derzeitiger State des Systems ungleich null
        {
            EmitSignal(nameof(PreExit)); //Signal Begin den aktuellen State zu verlassen
            state.OnExit(state.GetType().ToString()); //Verlassenfunktion
            EmitSignal(nameof(PostExit)); //Signal Erfolgreiches Verlassen des aktuellen States
        }

        LastState = CurrentState;
        CurrentState = state.GetType().ToString(); //nimmt den nächsten State aus Liste/Animationstree/ etc. an

        state = _state;
        EmitSignal(nameof(PreStar)); //Signal zu Einsetzen des Statewechsels
        state.OnStart(message); //Funktion des Statewechsels mit Information (wie Lebenspunkte, Spielfigurenanzahl, etc.) als message
        EmitSignal(nameof(PostStart)); //Signal Erfolgreiches Wechseln State
        state.OnUpdate();
    }

    //Anforderung eines Statewechsels von außen
    public void ChangeState(string stateName, Dictionary<string, object> message = null)
    {
        foreach (SimpleState _state in States) //geh alle States durch die in der Liste existieren
        {
            if (stateName == _state.GetType().ToString()) //wenn der aktuelle State, zu den gewechselt werden soll, gleich dem in der Liste ist
            {
                SetState(_state, message); //Wechsel die States
                return;
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
       if (state == null) 
        {  return; }
       state.UpdateState(delta);
    }
}
