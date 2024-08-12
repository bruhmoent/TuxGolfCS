using Godot;
using System;

public partial class Menu : Control
{
	public override void _Ready(){
	}

	public override void _Process(double delta){}

	private void _on_story_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/level_selection.tscn");
	}

	private void _on_settings_pressed()
	{
		//...
	}
	private void _on_exit_pressed()
	{
		GetTree().Quit();
	}

}
