using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void PlayerDiedEventHandler();

    // Constants for golf ball control
    private const float MaxForce = 1100.0f;
    private const float MinForce = 130.0f;
    private const float Gravity = 700.0f;
    private const float MaxIndicatorLength = 110.0f;
    private const int MaxHealth = 3;
    private const float DeathYThreshold = 600.0f;
    private const float Damping = 0.95f;
    private const float RollingSpeedMultiplier = 10.0f;
    private const float ForceMultiplier = 1.0f;
    private Vector2 viewportSize;

    // Variables for golf ball control
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private float shootingForce;
    private Line2D trajectoryLine;

    // Visual elements
    private Sprite2D playerSprite;
    private Sprite2D directionIndicator;
    private int currentHealth = MaxHealth;
    private CanvasLayer heartsCanvas;
    private Control container;
    private Vector2 rectSize;
    private float startingX = 0f;
    private bool isInvincible = false;
    private float invincibilityDuration = 1.0f;
    private Timer invincibilityTimer;
    private Camera2D camera;
    private float cameraSpeed = 1200.0f;
    private float directionIndicatorOpacity = 1.0f;
    private const float FadeOutSpeed = 2.0f;
    private const float FadeInSpeed = 4.0f;
    private bool wasAirborne = false;

    public enum Tags
    {
        Hurtable
    }

    public Tags Tag { get; set; } = Tags.Hurtable;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite2D>("PlayerSprite");
        directionIndicator = GetNode<Sprite2D>("DirectionIndicatorSprite");
        trajectoryLine = GetNode<Line2D>("TrajectoryLine");
        heartsCanvas = GetNode<CanvasLayer>("HeartsCanvas");
        container = heartsCanvas.GetNode<Control>("Control");

        UpdateDirectionIndicator();

        var heartTexture = GD.Load<Texture2D>("res://Scenes/UI/heart_texture.png");
        rectSize = new Vector2(heartTexture.GetWidth() * MaxHealth, heartTexture.GetHeight());

        heartsCanvas.Offset = new Vector2((GetViewportRect().Size.X - rectSize.X) / 2, rectSize.Y);
        startingX = heartsCanvas.Offset.X;

        invincibilityTimer = new Timer();
        AddChild(invincibilityTimer);
        invincibilityTimer.WaitTime = invincibilityDuration;
        invincibilityTimer.OneShot = true;
        invincibilityTimer.Connect("timeout", new Callable(this, "_onInvincibilityTimerTimeout"));
        viewportSize = GetViewportRect().Size;

        camera = GetTree().Root.GetNode<Camera2D>("TemplateLevel/Camera2D");

        SetProcess(true);
        UpdateHearts();
    }

    public void GetHurt(int multiplier = 1)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            currentHealth -= multiplier;
            UpdateHearts();

            invincibilityTimer.Start();
            FlashPlayerSprite();

            if (currentHealth <= 0)
            {
                EmitSignal("PlayerDied");
            }
        }
    }

    public override void _Process(double delta)
    {
        if (isInvincible)
        {
            playerSprite.Visible = !playerSprite.Visible;
        }

        if (GlobalPosition.Y > DeathYThreshold)
        {
            GetHurt(3);
        }
    }

    private void _onInvincibilityTimerTimeout()
    {
        isInvincible = false;
        playerSprite.Visible = true;
    }

    private void FlashPlayerSprite()
    {
        playerSprite.Visible = true;

        Timer flashTimer = new Timer();
        flashTimer.WaitTime = 0.1;
        flashTimer.OneShot = true;
        flashTimer.Connect("timeout", new Callable(this, "_onFlashTimerTimeout"));
        AddChild(flashTimer);

        flashTimer.Start();
    }

    private void _onFlashTimerTimeout()
    {
        playerSprite.Visible = !playerSprite.Visible;
    }

    private void UpdateHearts()
    {
        foreach (Node child in container.GetChildren())
        {
            if (child is Sprite2D)
            {
                child.QueueFree();
            }
        }

        for (int i = 1; i <= currentHealth; i++)
        {
            var heart = new Sprite2D();
            heart.Texture = GD.Load<Texture2D>("res://Scenes/UI/heart_texture.png");
            heart.Position = new Vector2(startingX + ((i - 1) * (rectSize.X / MaxHealth)), rectSize.Y);
            container.AddChild(heart);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMouseInput(delta);
        Vector2 velocity = Velocity;

        velocity.Y += Gravity * (float)delta;

        if (IsOnFloor())
        {
            if (velocity.Y > 0)
            {
                velocity.Y = 0;
            }
            velocity.X *= 0.75f;

            if (wasAirborne)
            {
                wasAirborne = false;
            }

            FadeInDirectionIndicator((float)delta);
        }
        else
        {
            FadeOutDirectionIndicator((float)delta);
            wasAirborne = true;
        }

        RotatePlayerSpriteBasedOnVelocity(velocity);
        Velocity = velocity;
        UpdateIndicatorLength();
        MoveAndSlide();
        UpdateTrajectory(delta);
        HandleCameraPeeking((float)delta);

    }

    private void FadeOutDirectionIndicator(float delta)
    {
        directionIndicatorOpacity = Mathf.Max(0, directionIndicatorOpacity - FadeOutSpeed * delta);
        directionIndicator.Modulate = new Color(1, 1, 1, directionIndicatorOpacity);
    }

    private void FadeInDirectionIndicator(float delta)
    {
        directionIndicatorOpacity = Mathf.Min(1, directionIndicatorOpacity + FadeInSpeed * delta);
        UpdateDirectionIndicatorOpacity();
    }

    private void RotatePlayerSpriteBasedOnVelocity(Vector2 velocity)
    {
        if (velocity.Length() > 0)
        {
            float angle = Mathf.Atan2(velocity.Y, velocity.X);
            float degrees = Mathf.RadToDeg(angle);

            playerSprite.RotationDegrees = degrees;
        }
    }

    private void UpdateTrajectory(double delta)
    {
        if (Input.IsActionPressed("ui_click") && IsOnFloor())
        {
            trajectoryLine.ClearPoints();

            Vector2 playerCenter = GlobalPosition;
            Vector2 direction = (dragStartPos - dragEndPos).Normalized();
            float force = shootingForce;
            Vector2 velocity = direction * force;

            Vector2 position = playerCenter;
            float stepSize = (float)delta;
            int numSteps = 200;

            bool wasOnFloor = true;
            for (int i = 0; i < numSteps; i++)
            {
                trajectoryLine.AddPoint(ToLocal(position));

                velocity.Y += Gravity * stepSize;

                if (wasOnFloor)
                {
                    if (velocity.Y > 0)
                    {
                        velocity.Y = 0;
                    }
                    velocity.X *= 0.75f;
                }

                position += velocity * stepSize;

                wasOnFloor = TestMove(new Transform2D(0, position), Vector2.Down);
            }
        }
        else
        {
            trajectoryLine.ClearPoints();
        }
    }

    private void HandleMouseInput(double delta)
    {
        if (IsOnFloor())
        {
            if (Input.IsActionPressed("ui_click"))
            {
                if (Input.IsActionJustPressed("ui_click"))
                {
                    dragStartPos = GetLocalMousePosition();
                }

                dragEndPos = GetLocalMousePosition();
                shootingForce = CalculateShootingForce();

                RotatePlayerSprite(dragStartPos, dragEndPos);
            }
            else if (Input.IsActionJustReleased("ui_click"))
            {
                ShootBall();
            }
        }
    }

    private float CalculateShootingForce()
    {
        Vector2 dragVector = dragStartPos - dragEndPos;
        float maxDistance = Mathf.Min(viewportSize.X, viewportSize.Y) * 0.5f;
        float normalizedDistance = dragVector.Length() / maxDistance;
        float force = Mathf.Lerp(MinForce, MaxForce, normalizedDistance * ForceMultiplier);
        return Mathf.Clamp(force, MinForce, MaxForce);
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

        playerSprite.RotationDegrees = degrees + 180.0f;
        directionIndicator.RotationDegrees = degrees + 180.0f;
    }

    private void UpdateIndicatorLength()
    {
        float indicatorLength = Mathf.Lerp(0, MaxIndicatorLength, shootingForce / MaxForce);
        directionIndicator.Scale = new Vector2(indicatorLength / 30, 1);
    }

    private void UpdateDirectionIndicatorOpacity()
    {
        directionIndicator.Modulate = new Color(1, 1, 1, directionIndicatorOpacity);
    }

    private void UpdateDirectionIndicator()
    {
        directionIndicator.Scale = new Vector2(1, 1);
        directionIndicatorOpacity = 1.0f;
        UpdateDirectionIndicatorOpacity();
    }

    private void HandleCameraPeeking(float delta)
    {
        Vector2 cameraMove = new Vector2();

        if (Input.IsActionPressed("camera_up"))
        {
            cameraMove.Y -= cameraSpeed * delta;
        }
        if (Input.IsActionPressed("camera_down"))
        {
            cameraMove.Y += cameraSpeed * delta;
        }
        if (Input.IsActionPressed("camera_left"))
        {
            cameraMove.X -= cameraSpeed * delta;
        }
        if (Input.IsActionPressed("camera_right"))
        {
            cameraMove.X += cameraSpeed * delta;
        }

        camera.Position += cameraMove;
    }
    public void _on_bump(Vector2 bumpVector)
    {
        shootingForce = 0;
    }
}