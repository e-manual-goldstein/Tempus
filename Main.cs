using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ball;

public class Main : Node2D
{
	public static bool Started;

	private Dictionary<BallType, int> _carams;
	private List<Ball> _pocketsThisShot = new List<Ball>();

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

	public int TurnScore => Math.Max(0, _carams.Values.Sum() - 1) + _pocketsThisShot.Sum(b => (int)b.BallType);
	public int ShotScore { get; set; }
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
		var shotScore = TurnScore;
		ShotScore += shotScore;
		ResetPocketed();
		GetNode<HUD>("HUD").UpdateScore(Score);
	}

	private void ResetPocketed()
	{
		foreach (var ball in _pocketsThisShot)
		{
			ball.Reset();
		};
	}

	public void TurnEnded()
	{
		ResetShot();
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

	private void BallPocketed(Ball ball)
	{
		GD.Print("Pocketed");
		switch (ball.BallType)
		{
			case BallType.White:
				ResetShot();
				break;
			case BallType.Yellow:
			case BallType.Red:
			case BallType.Orange:
				PocketBall(ball);
				break;
			default:
				break;
		}
		//ResetBall(ball);
	}

	private void PocketBall(Ball ball)
	{
		UpdateTurnScore(ball);
		ball.IsPocketed = true;
		ball.Visible = false;
		ball.Stop();
		//UpdateTurnScore(ball);
	}

	private void UpdateTurnScore(Ball ball)
	{
		_pocketsThisShot.Add(ball);
		GD.Print($"Updating Turn Score: {TurnScore}");
	}

	private void ResetShot()
	{
		ResetCarams();
		_pocketsThisShot.Clear();
	}
}
