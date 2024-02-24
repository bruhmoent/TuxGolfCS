using Godot;
using System;

public partial class SceneManager : Node2D
{
    public static SceneManager Instance
    {
        get
        {
            PackedScene packedScene = GD.Load<PackedScene>("res://Scenes/Levels/SceneManager.tscn");

            SceneManager instance = packedScene.Instantiate() as SceneManager;

            if (instance == null)
            {
                GD.PrintErr("Failed to instance SceneManager");
            }

            return instance;
        }
    }

    private string currentSceneName = "";

    public override void _Ready()
    {
        currentSceneName = GetTree().CurrentScene.Name;
    }

    public int GetCurrentSceneNumber()
    {
        if (currentSceneName.Contains("Level"))
        {
            string sceneNumberStr = currentSceneName.Substring(currentSceneName.Length - 6, 1);
            return int.Parse(sceneNumberStr);
        }
        else
        {
            return -1;
        }
    }
}
