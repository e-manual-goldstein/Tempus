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

	ColorRect Border => GetNode<ColorRect>("Border");
	MarginContainer Margin => GetNode<MarginContainer>("Margin");
	ColorRect Background => GetNode<ColorRect>("Margin/Background");
	Label NameLabel => GetNode<Label>("NameLabel");
	Label ScoreLabel => GetNode<Label>("ScoreLabel");

	public float GetHeight()
	{
		return Margin.Size.Y;
	}

	internal float GetWidth()
	{
		return Margin.Size.X;
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

	readonly string[] margins = new[] { "margin_top", "margin_left", "margin_right", "margin_bottom", };
	public void UpdateHighlighted(int playerId)
	{
		//int border = (playerId == PlayerId) ? 2 : 0;
		Color color = (playerId == PlayerId) ? Color.FromHtml("#6883b2") : Color.FromHtml("#6867b2");

		foreach (var margin in margins)
		{
			//Margin.AddThemeConstantOverride(margin, border);
			Background.Color = color;
		}
		
	}

	#endregion

	#region Logic

	public int PlayerId { get; }

	[Export]
	public string PlayerName { get; set; }

	[Export]
	public int PlayerScore { get; set; }

	public bool IsFirstShot { get; set; }

	internal void StartTurn()
	{
		MessageBox.PrintMessage($"{PlayerName}, Your Shot!");
		IsFirstShot = true;
	}

	#endregion


}
