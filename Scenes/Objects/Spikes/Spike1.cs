using Godot;
using System;
using System.Linq;

public partial class Spike1 : Area2D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
	private bool HasEnumTag(Node body, string tagName)
	{
		Type objectType = body.GetType();

		var tagProperty = objectType.GetProperty("Tag");
		if (tagProperty != null && tagProperty.PropertyType.IsEnum)
		{
			var enumValues = Enum.GetNames(tagProperty.PropertyType);
			return enumValues.Contains(tagName);
		}

		return false;
	}

   private void _on_body_entered(Node body)
{
	if (HasEnumTag(body, "Hurtable"))
	{
		if (body is Player player)
		{
			player.GetHurt(1);
		}
	}
}

}
