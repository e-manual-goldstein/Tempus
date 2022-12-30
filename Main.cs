using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ball;

public class Main : Node2D
{
	public static bool Started;

	private Dictionary<BallType, int> _carams;

	private Balls _balls;

	public override void _Ready()
	{
		_carams = BaseCarams();
		_balls = GetChildren().OfType<Balls>().Single();
	}


	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	public float Score;
	
	public void NewGame()
	{
		Started = true;
		Score = 0;

		var hud = GetNode<HUD>("HUD");
		hud.ShowMessage("Get Ready!");
		hud.UpdateScore(Score);
	}
	
	public void Caram(Ball ball)
	{
		_carams[ball.BallType] = 1;		
	}

	public void ShotTaken()
	{
		_balls.ShotTaken = true;
	}

	public void ShotEnded()
	{
		var shotScore = Math.Max(0, _carams.Values.Sum() - 1);
		ResetCarams();
		Score += shotScore;
		GetNode<HUD>("HUD").UpdateScore(Score);
	}

	private void ResetCarams()
	{
		GD.Print("Carams reset");
		_carams = BaseCarams();
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
	
	private Dictionary<BallType, int> BaseCarams()
	{
		return Enum.GetValues(typeof(BallType)).OfType<BallType>().ToDictionary(d => d, e => 0);
	}
}
