using Godot;

public partial class GenericDialogue : Control
{
	private RichTextLabel dialogueLabel;
	private Sprite2D spr1;
	private Sprite2D spr2;
	private CanvasLayer canvasLayer;
	private string[] dialogues;
	private int currentDialogueIndex = 0;
	private bool displayingText = false;
	private bool waitForNextPress = false;

	public override void _Ready()
	{
		canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		dialogueLabel = canvasLayer.GetNode<RichTextLabel>("DialogueLabel");
		spr1 = canvasLayer.GetNode<Sprite2D>("Sprite2D");
		spr2 = canvasLayer.GetNode<Sprite2D>("Sprite2D2");

		dialogues = new string[]
		{
			"[center]Welcome To [i]Galflands![/i] [b] Press enter to skip.[/b][/center]",
			"You can drag your mouse whilst holding the right mouse button to change the direction, angle and force of the shot.",
			"Also... you can pan around the camera using [b]WASD[/b].",
			"[center]Have fun? [i]...[/i][/center].",
		};

		float combinedWidth = spr1.Texture.GetWidth() * spr1.Scale.X + spr2.Texture.GetWidth() * spr2.Scale.X;

		canvasLayer.Offset = new Vector2((GetViewportRect().Size.X - combinedWidth) / 2, GetViewportRect().Size.Y - spr1.Texture.GetHeight() * spr1.Scale.Y);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_accept"))
		{
			if (!waitForNextPress)
			{
				waitForNextPress = true;

				currentDialogueIndex++;

				if (currentDialogueIndex < dialogues.Length)
				{
					ShowDialogue();
				}
				else
				{
					HideDialogue();
				}
			}
		}
		else
		{
			waitForNextPress = false;
		}
	}

	private void ShowDialogue()
	{
		if (currentDialogueIndex >= 0)
		{
			canvasLayer.Visible = true;
			displayingText = true;
			dialogueLabel.Text = dialogues[currentDialogueIndex];
		}
	}

	private void HideDialogue()
	{
		displayingText = false;
		dialogueLabel.Text = "";

		if (currentDialogueIndex < dialogues.Length - 1)
		{
		}
		else
		{
			canvasLayer.Visible = false;
		}
	}

	private void _on_area_2d_body_entered(Node body)
	{
		Node exampleLevel = GetTree().Root;
		CharacterBody2D character = FindCharacterNode(exampleLevel);

		if (character != null && body == character)
		{
			if (currentDialogueIndex >= 0)
				ShowDialogue();
		}
	}

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
}
