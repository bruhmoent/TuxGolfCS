using Godot;
using System;

public partial class BumperRight : Area2D
{
	public override void _Ready(){}

	public override void _Process(double delta){}
    private void _on_body_entered(Node body)
    {
        AnimatedSprite2D animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        float minVelocityMagnitude = 250.0f;

        if (body is CharacterBody2D player)
        {
            if (Math.Abs(player.Velocity.X) < minVelocityMagnitude)
            {
                float minVelocity = minVelocityMagnitude * 1.5f;

                if (player.Velocity.Y > 0)
                {
                    player.Velocity = new Vector2(minVelocity, -player.Velocity.Y);
                }
                else
                {
                    player.Velocity = new Vector2(minVelocity, player.Velocity.Y);
                }
            }
            else
            {
                if (player.Velocity.Y > 0)
                {
                    player.Velocity = new Vector2(player.Velocity.X * 1.5f, -player.Velocity.Y * 1.5f);
                }
                else
                {
                    player.Velocity = new Vector2(player.Velocity.X * 1.5f, player.Velocity.Y * 1.5f);
                }
            }

            Vector2 temp = new Vector2(player.Position.X + 2.5f, player.Position.Y);
            player.Position = temp;
        }


        animatedSprite.Play("default");
    }
}
