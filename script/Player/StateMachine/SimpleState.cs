using Godot;

using System.Collections.Generic;

public partial class SimpleState : Node
{
    //Statusvariablen für Kontroller der States
    private bool _hasBeenInitialized;

    //Events der States
    [Signal]
    public delegate void StateStartEventHandler(); //Start neuer State mit message (Stateinformationen)

    [Signal]
    public delegate void StateExitedEventHandler(); //wechsel zu neuen State, derzeitigen beenden

    //OnStart-Funktion -- ist vom Typ virtual, weil wir für jede Implementierung der Stateklasse OnStart überschreiben
    public virtual void
        OnStart(Dictionary<string, object> message) //message soll als Extrainfo dienen für die neuen States wie wenn verletzt wie viel Leben die Figur noch hat oder wenn gewonnen wie viele Figuren noch übrig sind
    {
        EmitSignal(SignalName.StateStart);
        _hasBeenInitialized = true;
    }

    //UpdateState-Funktion -- muss jeden Frame/ PhysicsUpdate von der Statemachine aufgerufen werden und überprüft ob ein Statewechsel stattfindet
    public virtual void UpdateState(double dt)
    {
    }

    //OnExit-Funktion -- gibt den nächsten State weiter über string nextState
    /// <summary>
    /// State wird verlassen
    /// </summary>
    /// <param name="nextState">Der nächste State</param>
    public virtual void OnExit(string nextState)
    {
        if (!_hasBeenInitialized)
        {
            return;
        }

        EmitSignal(SignalName.StateExited);
        _hasBeenInitialized = false;
    }
}