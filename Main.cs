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
	Cue Cue => GetNode<Cue>("Cue");

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

	public void HandlePlayerShot(float angle, float chargeStrength)
	{
		Vector2 directionVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		var velocity = directionVector * chargeStrength;
		EmitSignal(SignalName.CueBallStruck, velocity);
		Cue.Visible = false;

	}

	public void NewGame()
	{
		GD.Print("Starting New Game");
		Started = true;
		Scoreboard.StartNewGame(Players);
		var hud = GetNode<HUD>("HUD");
		hud.ShowMessage("Get Ready!");
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
}
