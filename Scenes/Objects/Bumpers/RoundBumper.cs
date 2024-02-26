using Godot;
using System;

public partial class RoundBumper : Area2D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
	private void _on_body_entered(Node body)
	{
        if (body is CharacterBody2D character)
        {
            Vector2 bumperCenter = GlobalPosition;
            Vector2 playerPosition = character.GlobalPosition;
            Vector2 reflectionVector = (playerPosition - bumperCenter).Normalized();

            Vector2 playerVelocity = character.Velocity;

            Vector2 newVelocity = playerVelocity - 2 * playerVelocity.Dot(reflectionVector) * reflectionVector;

            character.Velocity = newVelocity;
        }
    }
}
