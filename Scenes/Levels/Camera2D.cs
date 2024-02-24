using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{
    [Export]
    private float followSpeed = 0.1f;

    private CharacterBody2D player;

    public override void _Ready()
    {
        Node root = GetTree().Root;

        player = root.GetNodeOrNull<CharacterBody2D>("TemplateLevel/CharacterBody2D");

        if (player != null)
        {
            GD.Print("Player node found!");
        }
        else
        {
            GD.Print("Player node not found. Check the path.");
            PrintNodePaths(root);
        }
    }

    private Vector2 CustomLerp(Vector2 from, Vector2 to, float weight)
    {
        return from + (to - from) * weight;
    }

    private void PrintNodePaths(Node node, string currentPath = "")
    {
        GD.Print("Node Path: " + currentPath + node.Name);
        foreach (Node child in node.GetChildren())
        {
            PrintNodePaths(child, currentPath + node.Name + "/");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player != null)
        {
            Vector2 targetPosition = player.GlobalPosition;
            GlobalPosition = CustomLerp(GlobalPosition, targetPosition, followSpeed);
        }
    }
}
