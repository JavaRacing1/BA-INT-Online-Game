using Godot;

public partial class PlayerView : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 40;
    /// <summary>
    /// Anlegen einer Geschwindikeitsvariable
    /// </summary>

    private Vector2 _targetVelocity = Vector2.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("ui_up"))
        {
            direction.Y -= 1;
        }
        if (Input.IsActionPressed("ui_right"))
        {
            direction.X += 1;
        }
        if (Input.IsActionPressed("ui_down"))
        {
            direction.Y += 1;
        }
        if (Input.IsActionPressed("ui_left"))
        {
            direction.X -= 1;
        }
        if (direction != Vector2.Zero)
        {
            direction = direction.Normalized();
        }

        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Y = direction.Y * Speed;

        Velocity = _targetVelocity;
    }
}
