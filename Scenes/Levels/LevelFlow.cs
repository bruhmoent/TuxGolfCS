using Godot;
using System;
using System.Text.Json;

public static class LevelState
{
	public static bool[] level1 = new bool[3] { true, false, false }; // Unlocked, Completed, All-Coins
	public static bool[] level2 = new bool[3] { false, false, false };
	public static bool[] level3 = new bool[3] { false, false, false };
	public static bool[] level4 = new bool[3] { false, false, false };
	public static bool[] level5 = new bool[3] { false, false, false };
	
	 public static void SaveState()
	{
		var levelStates = new[]
		{
			level1, level2, level3, level4, level5
		};

		var json = System.Text.Json.JsonSerializer.Serialize(levelStates);
		var filePath = "user://level_state.json";

		using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
		{
			file.StoreString(json);
		}

		GD.Print("Level state saved!");
	}	
}

public partial class LevelFlow : Control
{
	private const int TotalLevels = 5;

	private static bool[] GetCurrentLevelState(int level)
	{
		return level switch
		{
			1 => LevelState.level1,
			2 => LevelState.level2,
			3 => LevelState.level3,
			4 => LevelState.level4,
			5 => LevelState.level5,
			_ => new bool[3] { false, false, false },
		};
	}

	public void LoadLevelState()
	{
		var filePath = "user://level_state.json";

		if (!FileAccess.FileExists(filePath))
		{
			GD.Print("No save file found.");
			return;
		}

		using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
		{
			var json = file.GetAsText();
			var levelStates = JsonSerializer.Deserialize<bool[][]>(json);

			LevelState.level1 = levelStates[0];
			LevelState.level2 = levelStates[1];
			LevelState.level3 = levelStates[2];
			LevelState.level4 = levelStates[3];
			LevelState.level5 = levelStates[4];
		}

		GD.Print("Level state loaded!");
		UpdateLevelButtons();
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
				levelButton.Text = $"{i}";
				if (isCompleted)
					levelButton.Text = "️√";
				
				levelButton.Disabled = false;
			}
			else
			{
				levelButton.Text = $"Locked";
				levelButton.Disabled = true;
			}
		}
	}

	public override void _Ready()
	{
		UpdateLevelButtons();
		LoadLevelState();
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
}
