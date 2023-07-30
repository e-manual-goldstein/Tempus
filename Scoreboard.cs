using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Scoreboard : Node2D
{

	public List<PlayerScorecard> Scorecards { get; set; } = new List<PlayerScorecard>();

	public void PlayerAdded(PlayerScorecard playerScorecard)
	{
		var y = Scorecards.Sum(s => s.GetHeight());

		Scorecards.Add(playerScorecard);
	}

	[Signal]
	public delegate void TurnEndedEventHandler(Balls balls);
	
	private Dictionary<BallType, int> _caroms;
	
	private int CaromScore()
	{
		return Math.Max(0, _caroms.Values.Sum() - 1);
	}

	private List<Ball> _pocketsThisShot = new List<Ball>();
	private int PocketScore()
	{
		return _pocketsThisShot.Sum(r => (int)r.BallType);
	}

	private List<BallType> _pocketsThisTurn = new List<BallType>();

	public int ShotScore()
	{
		return PocketScore() + CaromScore(); 
	}

	public int TurnScore { get; set; }
	public int TotalScore { get; set; }
	   
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

	public void UpdateScoreLabel(int score)
	{
		GetNode<Label>("Score").Text = score.ToString();
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
		ReplaceBall(ball, balls, Balls.GetStartPosition(ball.BallType), true);
	}

	private void ResetCaroms()
	{
		GD.Print("Caroms reset");
		_caroms = BaseCaroms();
	}

	public void ShotEnded(Balls balls)
	{
		GD.Print("End of Shot");
		TurnScore += ShotScore();
		UpdateScoreLabel(TotalScore + TurnScore);
		if (!ShotWasLegal() || !PointsScored())
		{
			EndTurn();
		}
		ResetPocketed(balls);
		ResetCaroms();
	}

	private void EndTurn()
	{
		GD.Print("End of Turn");
		if (!ShotWasLegal())
		{
			GD.Print($"Shot was illegal, lost {TurnScore} points");
		}
		else
		{
			TotalScore += TurnScore;
		}
		TurnScore = 0;
		EmitSignal(SignalName.TurnEnded);
	}

	private bool PointsScored()
	{
		return ShotScore() > 0;
	}

	private bool ShotWasLegal()
	{
		if (OneOrMoreBallsStruck() && !CueballWasPocketed())
		{
			GD.Print("Shot was legal");
			return true;
		}
		GD.Print("Shot was illegal");
		return false;
	}

	private bool OneOrMoreBallsStruck()
	{
		if (CaromScore() >= 0)
		{
			GD.Print("One or more balls was struck");
			return true;
		}
		GD.Print("No ball was struck");
		return false;
	}

	private bool CueballWasPocketed()
	{
		if (_pocketsThisShot.Any(b => b.IsCueball))
		{
			GD.Print("Cueball was pocketed");
		}
		return false;
	}

	private Dictionary<BallType, int> BaseCaroms()
	{
		return Enum.GetValues(typeof(BallType)).OfType<BallType>()
			.Where(r => r != BallType.White).ToDictionary(d => d, e => 0);
	}

	private void PocketBall(Ball ball)
	{
		ball.IsPocketed = true;
		ball = ReplaceBall(ball, this, GetSocketLocation(ball.BallType), !ball.IsCueball);
		UpdatePocketed(ball);
	}

	private Ball ReplaceBall(Ball ball, Node newParent, Vector2 newLocation, bool visible)
	{
		ball.Stop();
		PackedScene ballScene = (PackedScene)ResourceLoader.Load("res://Ball.tscn");
		var replacementBall = ballScene.Instantiate() as Ball;
		replacementBall.Clone(ball);
		replacementBall.Position = newLocation;//UpdateTurnScore(ball);
		replacementBall.Visible = visible;
		newParent.AddChild(replacementBall);
		ball.Visible = false;
		ball.GetParent().RemoveChild(ball);
		ball.QueueFree();
		return replacementBall;
	}

	private void PocketCueBall(Ball ball)
	{
		ball.IsPocketed = true;
		ball.Stop();
		ResetCaroms();
	}

	private void BallPocketed(Ball ball)
	{
		GD.Print($"Pocketed {ball}");
		switch (ball.BallType)
		{
			case BallType.White:
				//PocketCueBall(ball);
				//break;
			case BallType.Yellow:
			case BallType.Red:
			case BallType.Orange:
				CallDeferred("PocketBall", ball);
				break;
			default:
				break;
		}
	}

	private void UpdatePocketed(Ball ball)
	{
		_pocketsThisShot.Add(ball);
		_pocketsThisTurn.Add(ball.BallType);
		GD.Print($"Updating Turn Score: {TurnScore}");
	}
}
