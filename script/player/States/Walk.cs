using Godot;

public partial class Walk : State
{
    public override void PhysicProcess(double delta)
    {
        Vector2 velocity = Character.Velocity;

        //Bewegung nach links
        if (Input.IsActionPressed("walk_left"))
        {
            GD.Print("Move Left");              //Debugger-Code
            velocity.X = -Speed;                //Geschwindigkeit negativ, da Bewegung entgegen x-Achse
            Character.Velocity = velocity;        //Spieler mit der gewünschten Geschwindigkeit in gewünschte Richtung bewegen
            _ = Character.MoveAndSlide();         //Bewegung aktualisieren
        }
        //Bewegung nach rechts
        else if (Input.IsActionPressed("walk_right"))
        {
            GD.Print("Move Right");             //Debugger-Code
            velocity.X = Speed;                 //Geschwindigkeit mit der sich der Spieler bewegt berücksichtigen
            Character.Velocity = velocity;        //Spieler mit der gewünschten Geschwindigkeit in gewünschte Richtung bewegen
            _ = Character.MoveAndSlide();         //Bewegung aktualisieren
        }
        //Wechsel in den State idle, falls keine Bewegung mehr durchgeführt wird
        else if (!Input.IsActionPressed("walk_right") && !Input.IsActionPressed("walk_left"))
        {
            Character.StateMachine.TransitionTo("idle");
        }
        //Spieler in den State Inair versetzen falls eine der Bedingungen erfüllt
        else if (!Character.IsOnFloor())
        {
            Character.StateMachine.TransitionTo("inair");
        }
    }
}
