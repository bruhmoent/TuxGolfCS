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

    // Variables for golf ball control
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private float shootingForce;
    private const float Damping = 0.95f;

    // Visual elements
    private Sprite2D playerSprite;
    private Sprite2D directionIndicator;
    private int currentHealth = MaxHealth;
    private CanvasLayer heartsCanvas;
    private Control container;
    Vector2 rectSize;
    float startingX = 0f;
    private bool isInvincible = false;
    private float invincibilityDuration = 1.0f;
    private Timer invincibilityTimer;

    public enum Tags
    {
        Hurtable
    }
    public Tags Tag { get; set; } = Tags.Hurtable;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite2D>("PlayerSprite");
        directionIndicator = GetNode<Sprite2D>("DirectionIndicatorSprite");
        UpdateDirectionIndicator();
        heartsCanvas = GetNode<CanvasLayer>("HeartsCanvas");
        container = heartsCanvas.GetNode<Control>("Control");
        var heartTexture = GD.Load<Texture2D>("res://Scenes/UI/heart_texture.png");
        var heartWidth = heartTexture.GetWidth();
        var heartHeight = heartTexture.GetHeight();

        rectSize = new Vector2(heartWidth * MaxHealth, heartHeight);
        heartsCanvas.Offset = new Vector2((GetViewportRect().Size.X - rectSize.X) / 2, rectSize.Y);
        startingX = heartsCanvas.Offset.X;

        invincibilityTimer = new Timer();
        AddChild(invincibilityTimer);
        invincibilityTimer.WaitTime = invincibilityDuration;
        invincibilityTimer.OneShot = true;
        invincibilityTimer.Connect("timeout", new Callable(this, "_onInvincibilityTimerTimeout"));

        SetProcess(true);

        UpdateHearts();
    }

    private void GetHurt()
    {
        if (!isInvincible)
        {
            isInvincible = true;
            currentHealth--;
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