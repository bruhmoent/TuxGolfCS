using Godot;
using System;

public partial class Player : CharacterBody2D
{
    // Constants for golf ball control
    private const float MaxForce = 1100.0f;
    private const float MinForce = 130.0f;
    private const float Gravity = 700.0f;
    private const float MaxIndicatorLength = 110.0f;

    // Variables for golf ball control
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private float shootingForce;
    private const float Damping = 0.95f;

    private Sprite2D playerSprite;

    private Sprite2D directionIndicator;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite2D>("PlayerSprite");

        directionIndicator = GetNode<Sprite2D>("DirectionIndicatorSprite");

        UpdateDirectionIndicator();
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMouseInput(delta);
        Vector2 velocity = Velocity;

        velocity.Y += Gravity * (float)delta;

        if (IsOnFloor() && velocity.Y > 0)
        {
            velocity.Y = 0;
        }

        if (IsOnFloor())
        {
            velocity.X *= 0.95f;
        }
        else
        {
            velocity.X *= 0.99f;
        }
        Velocity = velocity;

        UpdateIndicatorLength();

        MoveAndSlide();
    }

    private void HandleMouseInput(double delta)
    {
        if (IsOnFloor())
        {
            if (Input.IsActionPressed("ui_click"))
            {
                if (Input.IsActionJustPressed("ui_click"))
                {
                    dragStartPos = GetGlobalMousePosition();
                }

                dragEndPos = GetGlobalMousePosition();

                shootingForce = Mathf.Clamp((dragStartPos - dragEndPos).Length(), MinForce, MaxForce);

                RotatePlayerSprite(dragStartPos, dragEndPos);
            }
            else if (Input.IsActionJustReleased("ui_click"))
            {
                ShootBall();
            }
        }
    }

    private void ShootBall()
    {
        Vector2 direction = (dragStartPos - dragEndPos).Normalized();

        Velocity = direction * shootingForce;
    }

    private void RotatePlayerSprite(Vector2 start, Vector2 end)
    {
        float angle = Mathf.Atan2(end.Y - start.Y, end.X - start.X);

        float degrees = Mathf.RadToDeg(angle);

        playerSprite.RotationDegrees = degrees;
        directionIndicator.RotationDegrees = degrees + 180.0f;
    }

    private void UpdateIndicatorLength()
    {
        float indicatorLength = Mathf.Lerp(0, MaxIndicatorLength, shootingForce / MaxForce);

        directionIndicator.Scale = new Vector2(indicatorLength / 30, 1);
    }

    private void UpdateDirectionIndicator()
    {
        directionIndicator.Scale = new Vector2(1, 1);
    }
    public void _on_bump(Vector2 bumpVector)
    {
        shootingForce = 0;
    }


}