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

	MarginContainer Highlighter => GetNode<MarginContainer>("Highlighter");
	Label NameLabel => GetNode<Label>("NameLabel");
	Label ScoreLabel => GetNode<Label>("ScoreLabel");

	public float GetHeight()
	{
		return Highlighter.Size.Y;
	}

	internal float GetWidth()
	{
		return Highlighter.Size.X;
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

	public void UpdateHighlighted(int playerId)
	{
		int border = (playerId == PlayerId) ? 5 : 0;
		Highlighter.AddThemeConstantOverride("margin_top", border);
		Highlighter.AddThemeConstantOverride("margin_left", border);
		Highlighter.AddThemeConstantOverride("margin_bottom", border);
		Highlighter.AddThemeConstantOverride("margin_right", border);
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
