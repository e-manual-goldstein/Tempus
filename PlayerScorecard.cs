using Godot;
using System;

public partial class PlayerScorecard : Node2D
{
	#region Lifecycle

	public override void _Ready()
	{
		PlayerId = Guid.NewGuid();
	}

	#endregion

	#region Layout
	ColorRect Background => GetNode<ColorRect>("Background");
	public float GetHeight()
	{
		return Background.Size.Y;
	}

	internal float GetWidth()
	{
		return Background.Size.X;
	}

	#endregion

	#region Logic
	public Guid PlayerId { get; private set; }

	[Export]
	public string PlayerName { get; set; }

	[Export]
	public int PlayerScore { get; set; }

	#endregion


}
