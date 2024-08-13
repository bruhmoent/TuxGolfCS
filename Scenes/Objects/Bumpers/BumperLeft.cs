using Godot;
using System;

public partial class BumperLeft : Area2D
{
	public override void _Ready(){}

	public override void _Process(double delta){}
	
	private void _on_body_entered(Node body)
	{
		AnimatedSprite2D animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (body is CharacterBody2D player)
		{
			if (player.Velocity.Y > 0)
			{
				player.Velocity = new Vector2(-player.Velocity.Y, -player.Velocity.Y * 1.5f);
			}
			else
			{
				player.Velocity = new Vector2(-player.Velocity.X, player.Velocity.Y * 1.5f);
			}
		}

		animatedSprite.Play("default");
	}
}
