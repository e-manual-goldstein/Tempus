using Godot;
using System;

public partial class PlayerScorecard : Node2D
{

	private static int playerId;
	public PlayerScorecard() 
	{
		PlayerId = ++playerId;
	}

	#region Lifecycle
	public override void _Ready()
	{
		NameLabel.Text = PlayerName;
		UpdateScoreLabel();
	}

	#endregion

	#region Layout
	
	ColorRect Background => GetNode<ColorRect>("Background");
	Label NameLabel => GetNode<Label>("NameLabel");
	Label ScoreLabel => GetNode<Label>("ScoreLabel");

	public float GetHeight()
	{
		return Background.Size.Y;
	}

	internal float GetWidth()
	{
		return Background.Size.X;
	}

	internal void AddTurnScore(int turnScore)
	{
		PlayerScore += turnScore;
		UpdateScoreLabel();
	}

	private void UpdateScoreLabel()
	{
		ScoreLabel.Text = PlayerScore.ToString();
	}

	#endregion

	#region Logic

	public int PlayerId { get; }

	[Export]
	public string PlayerName { get; set; }

	[Export]
	public int PlayerScore { get; set; }

	#endregion


}
