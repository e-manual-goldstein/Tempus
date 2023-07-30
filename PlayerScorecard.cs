using Godot;
using System;

public partial class PlayerScorecard : Node2D
{
	public Guid PlayerId { get; private set; }

	public override void _Ready()
	{
		PlayerId = Guid.NewGuid();
	}

	internal float GetHeight()
	{
		var rect = GetNode<ColorRect>("Background").Size.Y;
		return rect;
	}

	[Export]
	public string PlayerName { get; set; }

	[Export]
	public int PlayerScore { get; set; }

}
