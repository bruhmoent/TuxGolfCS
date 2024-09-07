using Godot;
using System;
using LevelIndex;
public partial class Level3 : Node2D
{
    public override void _Ready()
    {
        CurrentLevelIndex.levelIndex = 3;
        GD.Print("Current Level Index in Level3: " + CurrentLevelIndex.levelIndex);
    }

    public override void _Process(double delta) { }
}
