using Godot;

namespace HexBasedStrategy;

public partial class Camera : Camera2D
{
    [Export]
    public int Velocity = 15;

    [Export]
    public float ZoomSpeed = 0.05f;

    [Export]
    public float ZoomOutMax = 0.5f;

    [Export]
    public float ZoomInMax = 2.0f;

    private bool mouseWheelscrollingUp = false;
    private bool mouseWheelscrollingDown = false;

    float leftBound,
        rightBound,
        topBound,
        bottomBound;

    public override void _Ready() { }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("map_move_right"))
        {
            Position += new Vector2(Velocity, 0);
        }

        if (Input.IsActionPressed("map_move_left"))
        {
            Position += new Vector2(-Velocity, 0);
        }

        if (Input.IsActionPressed("map_move_up"))
        {
            Position += new Vector2(0, -Velocity);
        }

        if (Input.IsActionPressed("map_move_down"))
        {
            Position += new Vector2(0, Velocity);
        }

        if (Input.IsActionPressed("map_zoom_in") || mouseWheelscrollingUp)
        {
            if (Zoom.X <= ZoomInMax)
            {
                Zoom += new Vector2(ZoomSpeed, ZoomSpeed);
            }
        }

        if (Input.IsActionPressed("map_zoom_out") || mouseWheelscrollingDown)
        {
            if (Zoom.X >= ZoomOutMax)
            {
                Zoom -= new Vector2(ZoomSpeed, ZoomSpeed);
            }
        }

        if (Input.IsActionJustReleased("map_zoom_in_mouse"))
        {
            mouseWheelscrollingUp = true;
        }

        if (!Input.IsActionJustReleased("map_zoom_in_mouse"))
        {
            mouseWheelscrollingUp = false;
        }

        if (Input.IsActionJustReleased("map_zoom_out_mouse"))
        {
            mouseWheelscrollingDown = true;
        }

        if (!Input.IsActionJustReleased("map_zoom_out_mouse"))
        {
            mouseWheelscrollingDown = false;
        }
    }
}
