using Godot;

namespace HexBasedStrategy.Core;

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

    [Export]
    public int Padding = 100;

    private bool mouseWheelscrollingUp = false;
    private bool mouseWheelscrollingDown = false;

    float leftBound,
        rightBound,
        topBound,
        bottomBound;

    public override void _Ready()
    {
        GlobalEvents.MapGenerationCompleted += OnMapGenerationCompleted;
        GD.Print("Camera Ready!");
    }

    public override void _ExitTree()
    {
        GlobalEvents.MapGenerationCompleted -= OnMapGenerationCompleted;
    }

    public override void _PhysicsProcess(double delta)
    {
        var currentPosition = GetScreenCenterPosition();
        //GD.Print(
        //    $"limitRight={LimitRight}, limitLeft={LimitLeft}, limitTop={LimitTop}, limitBottom={LimitBottom}, lastPosition={lastPosition}, currentPosition={currentPosition}, globalPosition={GlobalPosition}, IsCameraLimitHit={IsCameraLimitHit(currentPosition)}"
        //);

        //GD.Print(
        //    $"leftBound={leftBound}, rightBound={rightBound}, topBound={topBound}, bottomBound={bottomBound}, currentPosition={currentPosition}, globalPosition={GlobalPosition}, padding={Padding}"
        //);

        if (Input.IsActionPressed("map_move_right") && GlobalPosition.X < rightBound)
        {
            Position += new Vector2(Velocity, 0);
        }

        if (Input.IsActionPressed("map_move_left") && GlobalPosition.X > leftBound)
        {
            Position += new Vector2(-Velocity, 0);
        }

        if (Input.IsActionPressed("map_move_up") && currentPosition.Y > topBound)
        {
            Position += new Vector2(0, -Velocity);
        }

        if (Input.IsActionPressed("map_move_down") && currentPosition.Y < bottomBound)
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

    private void OnMapGenerationCompleted(HexTileMap map)
    {
        GD.Print("OnMapGenerationCompleted called from Camera.");
        //leftBound = ToGlobal(map.MapToLocal(new Vector2I(0, 0))).X - Padding;
        //rightBound = ToGlobal(map.MapToLocal(new Vector2I(map.Width, 0))).X - Padding;
        //topBound = ToGlobal(map.MapToLocal(new Vector2I(0, 0))).Y - Padding;
        //bottomBound = ToGlobal(map.MapToLocal(new Vector2I(0, map.Height))).Y + Padding;

        leftBound = map.MapToLocal(new Vector2I(0, 0)).X + Padding;
        rightBound = map.MapToLocal(new Vector2I(map.Width, 0)).X - Padding;
        topBound = map.MapToLocal(new Vector2I(0, 0)).Y + Padding;
        bottomBound = map.MapToLocal(new Vector2I(0, map.Height)).Y - Padding;

        //LimitLeft = (int)map.MapToLocal(new Vector2I(0, 0)).X - Padding;
        //LimitRight = (int)map.MapToLocal(new Vector2I(map.Width, 0)).X + Padding;
        //LimitTop = (int)map.MapToLocal(new Vector2I(0, 0)).Y - Padding;
        //LimitBottom = (int)map.MapToLocal(new Vector2I(0, map.Height)).Y + Padding;
    }
}
