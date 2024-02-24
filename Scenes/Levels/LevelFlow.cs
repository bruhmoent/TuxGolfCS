using Godot;
using System;
public static class LevelState
{
    public static bool[] level1 = new bool[3] { true, false, false }; //Unlocked, Completed, All-Coins
    public static bool[] level2 = new bool[3] { false, false, false };
    public static bool[] level3 = new bool[3] { false, false, false };
    public static bool[] level4 = new bool[3] { false, false, false };
    public static bool[] level5 = new bool[3] { false, false, false };
}
public partial class LevelFlow : Control
{
    private const int TotalLevels = 5;
    private static bool[] GetCurrentLevelState(int level)
    {
        switch (level)
        {
            case 1:
                return LevelState.level1;
            case 2:
                return LevelState.level2;
            case 3:
                return LevelState.level3;
            case 4:
                return LevelState.level4;
            case 5:
                return LevelState.level5;
            default:
                return new bool[3] { false, false, false };
        }
    }

    private void UpdateLevelButtons()
    {
        for (int i = 1; i <= 5; i++)
        {
            bool[] levelState = GetCurrentLevelState(i);
            bool isUnlocked = levelState[0];
            bool isCompleted = levelState[1];

            Button levelButton = GetNode<Button>($"Level{i}");

            if (isUnlocked)
            {
                levelButton.Text = $"{i}" + (isCompleted ? " ✓" : "");
                levelButton.Disabled = false;
            }
            else
            {
                levelButton.Text = $"{i}\nLocked";
                levelButton.Disabled = true;
            }
        }
    }
    public void _on_level_pressed(int level)
    {
        bool[] levelState = GetCurrentLevelState(level);

        if (levelState[0])
        {
            GetTree().ChangeSceneToFile($"res://Scenes/Levels/Level{level}.tscn");
        }
        else
        {
            GD.Print($"Level {level} is locked. Complete the previous levels first.");
        }
    }

    private void _on_level_1_pressed()
    {
        _on_level_pressed(1);
    }
    private void _on_level_2_pressed()
    {
        _on_level_pressed(2);
    }

    private void _on_level_3_pressed()
    {
        _on_level_pressed(3);
    }

    private void _on_level_4_pressed()
    {
        _on_level_pressed(4);
    }

    private void _on_level_5_pressed()
    {
        _on_level_pressed(5);
    }
    public override void _Ready()
    {
        UpdateLevelButtons();
    }
}