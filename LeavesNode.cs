using Godot;
using System;

public partial class LeavesNode : Node2D
{
    private CanvasLayer leavesCanvasLayer;

    public override void _Ready()
	{
        leavesCanvasLayer = GetNode<CanvasLayer>("CanvasLayer");

        if (leavesCanvasLayer == null)
        {
            GD.PrintErr("CanvasLayer not found under LeavesNode.");
            return;
        }

        if (leavesCanvasLayer != null)
        {
            Vector2 screenSize = GetViewportRect().Size;

            float middleX = screenSize.X + 15f / 2;
            float middleY = screenSize.Y / 2;

            leavesCanvasLayer.Offset = new Vector2(middleX, middleY);
        }
    }

	public override void _Process(double delta){}
}
