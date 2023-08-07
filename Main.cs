using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using GodotBilliards;

public partial class Main : Node2D
{
	public static bool Started;

	[Signal]
	public delegate void CueBallStruckEventHandler(Vector2 velocity);

	public override void _Ready()
	{
		CreateNewPlayer("Kevin");
		CreateNewPlayer("Amy");
		CreateNewPlayer("Dog");
	}

	Scoreboard Scoreboard => GetNode<Scoreboard>("Background/Scoreboard");
	//Cue Cue => GetChildren().OfType<Cue>().SingleOrDefault();
	Balls Balls => GetNode<Balls>("Background/Border/Table/Balls");

	public Dictionary<int, PlayerScorecard> Players { get; set; } = new Dictionary<int, PlayerScorecard>();

	private PlayerScorecard CreateNewPlayer(string playerName, int playerScore = 0)
	{
		PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://PlayerScorecard.tscn");
		var player = playerScene.Instantiate() as PlayerScorecard;
		player.PlayerName = playerName;
		player.PlayerScore = playerScore;
		Players[player.PlayerId] = player;
		Scoreboard.PlayerAdded(player, Players.Count);
		return player;
	}

	//Move this to Balls.cs
	public void HandlePlayerShot(Cue cue)
	{
		Vector2 directionVector = new Vector2(Mathf.Cos(cue.CueAngle), Mathf.Sin(cue.CueAngle));
		EmitSignal(SignalName.CueBallStruck, directionVector * cue.ChargeStrength * 10);
		RemoveCueFromScene(cue);
	}

	public void NewGame()
	{
		GD.Print("Starting New Game");
		Started = true;
		Scoreboard.StartNewGame(Players);
		Balls.ShotEnded += AddCueToScene;
		var hud = GetNode<HUD>("HUD");
		hud.ShowMessage("Get Ready!");
		AddCueToScene(Balls);
	}

	private void AddCueToScene(Balls balls)
	{
		PackedScene cueScene = (PackedScene)ResourceLoader.Load("res://Cue.tscn");
		var cue = cueScene.Instantiate() as Cue;
		cue.Name = "Cue";
		cue.Shoot += HandlePlayerShot;
		cue.ZIndex = 1;
		cue.TargetCueBall(balls.Cueball.Position);
		AddChild(cue);
	}

	private void RemoveCueFromScene(Cue cue)
	{
		cue.Shoot -= HandlePlayerShot;
		//Balls.ShotEnded -= AddCueToScene;
		RemoveChild(cue);
		cue.QueueFree();
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
}
