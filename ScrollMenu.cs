using Godot;
using System;

public partial class ScrollMenu : ParallaxBackground
{
	public override void _Process(double delta)
	{
		ScrollBaseOffset -= new Vector2(0.5f, 0);
	}
}
