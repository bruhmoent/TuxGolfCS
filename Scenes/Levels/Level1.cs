using Godot;
using System;
using LevelIndex;
public partial class Level1 : Node2D
{
    public override void _Ready()
    {
        int currentLevelIndex = CurrentLevelIndex.levelIndex;

        GD.Print("Current Level Index in Level1: " + currentLevelIndex);
    }

    public override void _Process(double delta)
    {
        CurrentLevelIndex.levelIndex = 1;
    }
}