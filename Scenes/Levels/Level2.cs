using Godot;
using System;
using LevelIndex;
public partial class Level2 : Node2D
{
	public override void _Ready()
	{
		CurrentLevelIndex.levelIndex = 2;
		GD.Print("Current Level Index in Level2: " + CurrentLevelIndex.levelIndex);
	}
	
	public override void _Process(double delta){}
}
