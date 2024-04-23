using Godot;
using System.Collections.Generic;
public partial class SimpleState : Node
{
    //Statusvariablen für Kontroller der States
    private bool HasBeenInitialized = false;

    private bool OnUpdateHasFired = false;

    //Events der States
    [Signal] public delegate void StateStart(); //Start neuer State mit message (Stateinformationen)

    [Signal] public delegate void StateUpdate(); //ubdaten des aktuellen State

    [Signal] public delegate void StateExited(); //wechsel zu neuen State, derzeitigen beenden

    //OnStart-Funktion -- ist vom Typ virtual, weil wir für jede Implementierung der Stateklasse OnStart überschreiben
    public virtual void OnStart(Dictionary<string, object> message) //message soll als Extrainfo dienen für die neuen States wie wenn verletzt wie viel Leben die Figur noch hat oder wenn gewonnen wie viele Figuren noch übrig sind
    {
        EmitSignal(nameof(StateStart));
        HasBeenInitialized = true;
    }
    //OnUbdate-Funktion -- soll nur ein mal aufgerufen werden, überprüft ob OnStart zuvor aufgerufen ist und schaltet den Signal Update State ein
    public virtual void OnUpdate()
    {
        if (!HasBeenInitialized) { return; }
        EmitSignal(nameof(StateUpdate));
        OnUpdateHasFired = true;
    }

    //UpdateState-Funktion -- muss jeden Frame/ PhysicsUpdate von der Statemachine aufgerufen werden und überprüft ob ein Statewechsel stattfindet
    public virtual void UpdateState(float dt)
    {
        if (!OnUpdateHasFired) { return; }
    }

    //OnExit-Funktion -- gibt den nächsten State weiter über string nextState
    public virtual void OnExit(string nextState)
    {
       if (!HasBeenInitialized)
        { return; }
       EmitSignal(nameof(StateExited));
        HasBeenInitialized = false;
        OnUpdateHasFired = false;
    }

}
