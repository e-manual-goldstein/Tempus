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
		//System.Diagnostics.Debugger.Launch();
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
