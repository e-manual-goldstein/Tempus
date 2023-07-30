using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Scoreboard : Node2D
{
	#region Lifecycle

	public override void _Ready()
	{
		Debugger.Launch();
		_caroms = BaseCaroms();
		_headerHeight = Background.Size.Y;
		_yellowSocket = GetNode<Area2D>("YellowSocket").Position;
		_redSocket = GetNode<Area2D>("RedSocket").Position;
		_orangeSocket = GetNode<Area2D>("OrangeSocket").Position;
	}

	public override void _Process(double delta)
	{
	}

	#endregion

	#region Layout

	Vector2 _yellowSocket;
	Vector2 _redSocket;
	Vector2 _orangeSocket;
	float _headerHeight;
	ColorRect Background => GetNode<ColorRect>("Background");

	public List<PlayerScorecard> Players { get; set; } = new List<PlayerScorecard>();

	public void UpdateScoreLabel(int score)
	{
		GetNode<Label>("Score").Text = score.ToString();
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

	public void PlayerAdded(PlayerScorecard playerScorecard)
	{
		var y = Players.Sum(s => s.GetHeight()) + _headerHeight;
		var x = (Background.Size.X - playerScorecard.GetWidth()) / 2;
		playerScorecard.Position = new Vector2(x, y);
		LengthenScoreboard(y);
		AddChild(playerScorecard);
		Players.Add(playerScorecard);		
	}

	private void LengthenScoreboard(float lengthenBy)
	{
		var currentSize = Background.Size;
		var newSize = new Vector2(currentSize.X, currentSize.Y + lengthenBy);
		Background.SetSize(newSize);
	}

	#endregion

	#region Logic
	private Dictionary<BallType, int> _caroms;
	private List<Ball> _pocketsThisShot = new List<Ball>();
	private List<BallType> _pocketsThisTurn = new List<BallType>();

	public int TurnScore { get; set; }
	public int TotalScore { get; set; }

	[Signal]
	public delegate void TurnEndedEventHandler(Balls balls);

	private int CaromScore()
	{
		return Math.Max(0, _caroms.Values.Sum() - 1);
	}

	private int PocketScore()
	{
		return _pocketsThisShot.Sum(r => (int)r.BallType);
	}

	public int ShotScore()
	{
		return PocketScore() + CaromScore();
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

	#endregion
	
}
