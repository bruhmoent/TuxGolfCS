using Godot;
using System;
using LevelIndex;
public partial class Level1 : Node2D
{
	public override void _Ready()
	{
		CurrentLevelIndex.levelIndex = 1;
		GD.Print("Current Level Index in Level1: " + CurrentLevelIndex.levelIndex);
	}
	
	public override void _Process(double delta){}
}
