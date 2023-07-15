using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using GodotBilliards;

public partial class Main : Node2D
{
	public static bool Started;


	public override void _Ready()
	{
		//System.Diagnostics.Debugger.Launch();
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
