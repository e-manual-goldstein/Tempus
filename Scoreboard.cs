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
		//Debugger.Launch();
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

	public void UpdateScoreLabel(int score)
	{
		GetNode<Label>("TurnScoreLabel").Text = score.ToString();
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

	private Ball ReplaceBall(Ball ball, Node newParent, Vector2 newLocation, bool visible, bool attachSignal = false)
	{
		ball.Stop();
		PackedScene ballScene = (PackedScene)ResourceLoader.Load("res://Ball.tscn");
		var replacementBall = ballScene.Instantiate() as Ball;
		replacementBall.Clone(ball);
		replacementBall.Position = newLocation;//UpdateTurnScore(ball);
		replacementBall.Visible = visible;
		if (attachSignal)
		{
			replacementBall.Carom += Carom;
		}
		newParent.AddChild(replacementBall);
		ball.Visible = false;
		ball.GetParent().RemoveChild(ball);
		ball.QueueFree();
		return replacementBall;
	}

	public void PlayerAdded(PlayerScorecard playerScorecard, int playerNumber)
	{
		var y = (playerNumber - 1) * playerScorecard.GetHeight() + _headerHeight;
		var x = (Background.Size.X - playerScorecard.GetWidth()) / 2;
		playerScorecard.Position = new Vector2(x, y + x);
		LengthenScoreboard(playerScorecard.GetHeight(), x);
		AddChild(playerScorecard);
	}

	private void LengthenScoreboard(float lengthenBy, float border = 0)
	{
		var currentSize = Background.Size;
		var newSize = new Vector2(currentSize.X, currentSize.Y + lengthenBy + border);
		Background.SetSize(newSize);
	}

	#endregion

	#region Logic

	private int _currentPlayerId;
	Dictionary<int, PlayerScorecard> _players;    
	private Dictionary<BallType, int> _caroms;
	private List<Ball> _pocketsThisShot = new List<Ball>();
	private List<BallType> _pocketsThisTurn = new List<BallType>();

	public int TurnScore { get; set; }
	//public int TotalScore { get; set; }
	public bool FoulCommitted { get; set; }

	[Signal]
	public delegate void TurnEndedEventHandler(Balls balls);

	internal void StartNewGame(Dictionary<int, PlayerScorecard> players)
	{
		_players = players;
		_currentPlayerId = _players.Keys.First();
		UpdateHighlighted();
		CurrentPlayer().StartTurn();
	}

	private int GetNextPlayerId()
	{
		return _players.TryGetValue(_currentPlayerId + 1, out var player) ? player.PlayerId : _players.Min(p => p.Key);
	}

	private int CaromScore()
	{
		return Math.Max(0, CaromCount() - 1);
	}

	//The total number of balls that the cueball has hit on this shot
	private int CaromCount()
	{
		return _caroms.Values.Sum();
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
		if (CurrentPlayer().IsFirstShot && CaromCount() == 0)
		{
			if (ball.BallType != BallType.Red)
			{
				MessageBox.PrintMessage("Foul!");
				FoulCommitted = true;
			}
		}
		_caroms[ball.BallType] = 1;
		if (CaromScore() > 0)
		{
			MessageBox.PrintMessage("Carom!");
		}
			
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
		ReplaceBall(ball, balls, Balls.GetStartPosition(ball.BallType), true, ball.IsCueball);
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
		MessageBox.PrintMessage($"Scored {TurnScore} this turn");
		UpdateScoreLabel(TurnScore);
		var shotWasLegal = ShotWasLegal();

		CurrentPlayer().IsFirstShot = false;
		if (!shotWasLegal || !PointsScored())
		{
			EndTurn(shotWasLegal);
		}
		ResetPocketed(balls);
		ResetCaroms();
		FoulCommitted = false;
	}

	private void EndTurn(bool shotWasLegal)
	{
		MessageBox.PrintMessage("End of Turn");
		if (!shotWasLegal)
		{
			MessageBox.PrintMessage($"Foul committed, lost {TurnScore} points");
		}
		else
		{
			UpdatePlayerScore(TurnScore);
		}
		SelectNextPlayer();
		TurnScore = 0;
		UpdateScoreLabel(TurnScore);

		EmitSignal(SignalName.TurnEnded);
	}

	private void SelectNextPlayer()
	{
		_currentPlayerId = GetNextPlayerId();
		UpdateHighlighted();
		CurrentPlayer().StartTurn();
	}

	private PlayerScorecard CurrentPlayer()
	{
		return _players[_currentPlayerId];
	}

	private void UpdateHighlighted()
	{
		foreach (var (_, player) in _players)
		{
			player.UpdateHighlighted(_currentPlayerId);
		}
	}

	private void UpdatePlayerScore(int turnScore)
	{
		CurrentPlayer().AddTurnScore(turnScore);
	}

	private bool PointsScored()
	{
		return ShotScore() > 0;
	}

	private bool ShotWasLegal()
	{
		if (OneOrMoreBallsStruck() && !CueballWasPocketed() && !FoulCommitted)
		{
			GD.Print("Shot was legal");
			return true;
		}
		GD.Print("Shot was illegal");
		return false;
	}

	private bool OneOrMoreBallsStruck()
	{
		if (CaromCount() > 0)
		{
			GD.Print("One or more balls was struck");
			return true;
		}
		MessageBox.PrintMessage("No ball was struck");
		return false;
	}

	private bool CueballWasPocketed()
	{
		if (_pocketsThisShot.Any(b => b.IsCueball))
		{
			MessageBox.PrintMessage("Cueball was pocketed");
			return true;
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
				CallDeferred(nameof(PocketBall), ball);
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
