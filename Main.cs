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

		var scoreboard = GetNode<Scoreboard>("Background/Scoreboard");

		var playerOne = CreateNewPlayer("Kevin", 12);
		var playerTwo = CreateNewPlayer("Amy", 9);
		
		scoreboard.PlayerAdded(playerOne);
		scoreboard.PlayerAdded(playerTwo);

	}

	private PlayerScorecard CreateNewPlayer(string playerName, int playerScore = 0)
	{
		PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://PlayerScorecard.tscn");
		var player = playerScene.Instantiate() as PlayerScorecard;
		player.PlayerName = playerName;
		player.PlayerScore = playerScore;
		return player;
	}

	public override void _Input(InputEvent @event)
	{
		if (Started && @event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.IsPressed())
			{
				GD.Print($"Shot Taken {eventMouseButton.Position}");
				EmitSignal(SignalName.CueBallStruck, eventMouseButton.Position);                
			}
		}
	}    

	public void NewGame()
	{
		GD.Print("Starting New Game");
		Started = true;
		var hud = GetNode<HUD>("HUD");
		hud.ShowMessage("Get Ready!");
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
}
