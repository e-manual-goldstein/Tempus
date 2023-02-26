using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ball;

public partial class Main : Node2D
{
	public static bool Started;

	private Dictionary<BallType, int> _caroms;
	private List<Ball> _pocketsThisShot = new List<Ball>();

	private Balls _balls;

	public override void _Ready()
	{
		_caroms = BaseCaroms();
		_balls = GetChildren().OfType<Balls>().Single();
	}

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	public float Score;

	public int TurnScore { get; set; }
	public int ShotScore => Math.Max(0, _caroms.Values.Sum() - 1) + _pocketsThisShot.Sum(b => (int)b.BallType);

	public void NewGame()
	{
		GD.Print("Starting New Game");
		Started = true;
		Score = 0;
		var hud = GetNode<HUD>("HUD");
		hud.ShowMessage("Get Ready!");
		hud.UpdateScore(Score);
	}
	
	public void Carom(Ball ball)
	{
		GD.Print("Carom!");
		_caroms[ball.BallType] = 1;		
	}

	public void ShotTaken()
	{
		GD.Print("Shot Taken");
		_balls.ShotTaken = true;
	}

	public void ShotEnded()
	{
		GD.Print("Shot Ended");        
		CallDeferred("ResetPocketed");
		GetNode<HUD>("HUD").UpdateScore(Score);		
	}

	private void ResetPocketed()
	{
		foreach (var ball in _pocketsThisShot)
		{
			GD.Print($"Resetting {ball}");
			ball.Reset();
		};
	}

	public void TurnEnded()
	{
		ResetShot();		
	}

	private void ResetCaroms()
	{
		GD.Print("Caroms reset");
		_caroms = BaseCaroms();
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
	
	private Dictionary<BallType, int> BaseCaroms()
	{
		return Enum.GetValues(typeof(BallType)).OfType<BallType>().ToDictionary(d => d, e => 0);
	}

	private void BallPocketed(Ball ball)
	{
		GD.Print($"Pocketed {ball}");
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
		GD.Print("Updating Turn Score");
		UpdateShotScore(ball);
		ball.IsPocketed = true;
		ball.Visible = false;
		ball.Stop();
		//UpdateTurnScore(ball);
	}

	private void UpdateShotScore(Ball ball)
	{
		_pocketsThisShot.Add(ball);
		GD.Print($"Updating Turn Score: {TurnScore}");
	}

	private void ResetShot()
	{
		ResetCaroms();
		_pocketsThisShot.Clear();
	}
}
