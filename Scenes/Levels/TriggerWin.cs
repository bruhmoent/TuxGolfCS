using Godot;
using System;
using LevelIndex;
public partial class TriggerWin : Node2D
{
    public override void _Ready(){}

    public override void _Process(double delta){}

    private void _on_area_2d_body_entered(Node body)
    {
        MarkCurrentLevelComplete();
    }

    private void MarkCurrentLevelComplete()
    {
        int currentSceneNumber = CurrentLevelIndex.levelIndex;

        MarkLevelComplete(currentSceneNumber);
    }

    private void MarkLevelComplete(int level)
    {
        switch (level)
        {
            case 1:
                LevelState.level1[1] = true;
                LevelState.level2[0] = true;
                break;
            case 2:
                LevelState.level2[1] = true;
                LevelState.level3[0] = true;
                break;
            case 3:
                LevelState.level3[1] = true;
                LevelState.level4[0] = true;
                break;
            case 4:
                LevelState.level4[1] = true;
                LevelState.level5[0] = true;
                break;
            case 5:
                LevelState.level5[1] = true;
                break;
            default:
                GD.PrintErr($"Invalid level: {level}");
                break;
        }

        CallDeferred(nameof(PlayWinSoundAndChangeScene), "res://Scenes/Levels/level_selection.tscn");
    }
    private void PlayWinSoundAndChangeScene(string scenePath)
    {
        AudioStreamPlayer2D winSoundPlayer = GetNode<AudioStreamPlayer2D>("WinSoundPlayer");
        winSoundPlayer.GetParent()?.RemoveChild(winSoundPlayer);
        GetTree().Root.AddChild(winSoundPlayer);
        winSoundPlayer.Play();
        GetTree().ChangeSceneToFile("res://Scenes/Levels/level_selection.tscn");

    }
}