using Godot;
using System;

public partial class CoinDisplay : CanvasLayer
{
	public override void _Ready()
	{
		Vector2 RectPosition = new Vector2(20, 20);
	}

	public override void _Process(double delta)
	{
		Label coinLabel = GetNode<Label>("Label");
		coinLabel.Text = "    " + DataCoin.CoinGlobal.CoinCount;

		SetProcessInput(true);
	}

	public override void _Input(InputEvent @event){}
}
