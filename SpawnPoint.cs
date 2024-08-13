using Godot;
using System;

public partial class SpawnPoint : Node2D
{
	[Signal]
	public delegate void PlayerDiedEventHandler();

	private CharacterBody2D FindCharacterNode(Node node)
	{
		if (node is CharacterBody2D)
		{
			return (CharacterBody2D)node;
		}

		foreach (Node child in node.GetChildren())
		{
			CharacterBody2D result = FindCharacterNode(child);
			if (result != null)
			{
				return result;
			}
		}

		return null;
	}

	public override void _Ready()
	{
		Node exampleLevel = GetTree().Root;
		var player = FindCharacterNode(exampleLevel);
		if (player != null)
		{
			player.Connect("PlayerDied", new Callable(this, "_on_player_died"));
		}
	}

	public override void _Process(double delta){}

	private void _on_player_died()
	{
		GetTree().CreateTimer(0.5f).Connect("timeout", new Callable(this, "_on_reset_timer_timeout"));
	}

	private void _on_reset_timer_timeout()
	{
		GetTree().ReloadCurrentScene();
	}
}
