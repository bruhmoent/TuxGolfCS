using Godot;
using System;

public partial class ParallaxBackground : Godot.ParallaxBackground
{
    private CharacterBody2D player;
    private Camera2D camera;

    public override void _Ready()
    {
        player = GetNodeOrNull<CharacterBody2D>("/root/MainScene/TemplateLevel/CharacterBody2D");

        camera = GetNodeOrNull<Camera2D>("/root/MainScene/TemplateLevel/Camera2D");

        if (player != null && camera != null)
        {
            GD.Print("Player and Camera2D nodes found!");
        }
        else
        {
            GD.Print("Player or Camera2D node not found. Check the paths.");
            PrintNodePaths(GetTree().Root);
        }
    }

    private void PrintNodePaths(Node node, string currentPath = "")
    {
        GD.Print("Node Path: " + currentPath + node.Name);
        foreach (Node child in node.GetChildren())
        {
            PrintNodePaths(child, currentPath + node.Name + "/");
        }
    }

    public override void _Process(double delta)
    {
        if (player != null && camera != null)
        {
            Vector2 parallaxScroll = player.Velocity * (float)delta;

            Offset += parallaxScroll;

            Offset = camera.GlobalPosition / 2.0f;
        }
    }
}
