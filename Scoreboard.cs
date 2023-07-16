using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Scoreboard : Node2D
{

	private Dictionary<BallType, int> _caroms;
	private List<Ball> _pocketsThisShot = new List<Ball>();

	public float Score;

	public int TurnScore { get; set; }
	public int ShotScore => Math.Max(0, _caroms.Values.Sum() - 1) + _pocketsThisShot.Sum(b => (int)b.BallType);

   
	public override void _Ready()
	{
		Debugger.Launch();
		_caroms = BaseCaroms();

		_yellowSocket = GetNode<Area2D>("YellowSocket").Position;
		_redSocket = GetNode<Area2D>("RedSocket").Position;
		_orangeSocket = GetNode<Area2D>("OrangeSocket").Position;
	}

	Vector2 _yellowSocket;
	Vector2 _redSocket;
	Vector2 _orangeSocket;

	public void UpdateScore(float score)
	{
		GetNode<Label>("Score").Text = ((int)score).ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector2 GetSocketLocation(BallType ballType)
	{
		switch (ballType)
		{
			case BallType.Yellow:
				return _yellowSocket;
			case BallType.Red:
				return _redSocket;
			case BallType.Orange:
				return _orangeSocket;
			default:
			case BallType.White:
				break;				
		}
		return default;
	}

	public void Carom(Ball ball)
	{
		GD.Print("Carom!");
		_caroms[ball.BallType] = 1;
	}

	private void ResetPocketed(Balls balls)
	{
		foreach (var ball in _pocketsThisShot)
		{
			ResetBall(ball, balls);
		};
		_pocketsThisShot.Clear();
	}

	private void ResetBall(Ball ball, Balls balls)
	{
		GD.Print($"Resetting {ball}");
		ReplaceBall(ball, balls, Balls.GetStartPosition(ball.BallType));
	}

	private void ResetCaroms()
	{
		GD.Print("Caroms reset");
		_caroms = BaseCaroms();
	}

	public void ShotEnded(Balls balls)
	{
		ResetPocketed(balls);
		UpdateScore(Score);
	}

	private Dictionary<BallType, int> BaseCaroms()
	{
		return Enum.GetValues(typeof(BallType)).OfType<BallType>().ToDictionary(d => d, e => 0);
	}

	private void PocketBall(Ball ball)
	{
		GD.Print("Updating Turn Score");
		ball.IsPocketed = true;
		var replacementBall = ReplaceBall(ball, this, GetSocketLocation(ball.BallType));
		UpdateShotScore(replacementBall);
	}

	private Ball ReplaceBall(Ball ball, Node newParent, Vector2 newLocation)
	{
		ball.Stop();
		PackedScene ballScene = (PackedScene)ResourceLoader.Load("res://Ball.tscn");
		var replacementBall = ballScene.Instantiate() as Ball;
		replacementBall.Clone(ball);
		replacementBall.Position = newLocation;//UpdateTurnScore(ball);
		newParent.AddChild(replacementBall);
		ball.Visible = false;
		RemoveChild(ball);
		ball.QueueFree();
		return replacementBall;
	}

	private void PocketCueBall(Ball ball)
	{
		ball.IsPocketed = true;
		ball.Stop();
		ResetCaroms();
		//UpdateShotScore(ball);
	}

	private void BallPocketed(Ball ball)
	{
		GD.Print($"Pocketed {ball}");
		switch (ball.BallType)
		{
			case BallType.White:
				PocketCueBall(ball);
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
