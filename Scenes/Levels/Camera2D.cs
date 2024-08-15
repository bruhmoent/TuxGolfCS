using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{
    [Export]
    private float followSpeed = 7.0f;

    private CharacterBody2D player;

    public override void _Ready()
    {
        Node root = GetTree().Root;
        player = root.GetNodeOrNull<CharacterBody2D>("TemplateLevel/CharacterBody2D");
    }

    private Vector2 CustomLerp(Vector2 from, Vector2 to, float weight)
    {
        return from + (to - from) * weight;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player != null)
        {
            Vector2 targetPosition = player.GlobalPosition;
            GlobalPosition = CustomLerp(GlobalPosition, targetPosition, followSpeed * (float)delta);
        }
    }
}
