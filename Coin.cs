using Godot;
using System;

public partial class Coin : Node2D
{
    public override void _Ready(){}

    public override void _Process(double delta){}
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

    private AudioStreamPlayer2D FindStreamNode(Node node, string name)
    {
        if (node is AudioStreamPlayer2D && node.Name == name)
        {
            return (AudioStreamPlayer2D)node;
        }

        foreach (Node child in node.GetChildren())
        {
            AudioStreamPlayer2D result = FindStreamNode(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private void _on_area_2d_body_entered(Node body)
    {
        Node exampleLevel = GetTree().Root;

        CharacterBody2D character = FindCharacterNode(exampleLevel);
        AudioStreamPlayer2D coinSound = FindStreamNode(exampleLevel, "PickUp");


        if (character != null && body == character)
        {
            if(coinSound == null)
            {
                GD.Print("Coin sound not found");
                return;
            }

            coinSound.Play();

            DataCoin.CoinGlobal.CoinCount++;

            QueueFree();
        }
        else
        {
            GD.Print("Not a character");
        }
    }
}