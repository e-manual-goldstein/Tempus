using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Balls : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	static Dictionary<BallType, Vector2> _startPositions;

	[Signal]
	public delegate void ShotEndedEventHandler(Balls balls);

	public Ball[] AllBalls => GetChildren().OfType<Ball>().ToArray();

	public static int BallIdCounter;
	public bool ShotTaken { get; set; }
	
	public override void _Ready()
	{
		Debugger.Launch();
		_startPositions = AllBalls.ToDictionary(d => d.BallType, r => r.Position);
	}

	public override void _Process(double delta)
	{
		if (ShotTaken)
		{
			if (AllBalls.Any() && AllBalls.All(b => b.LinearVelocity.Length() < 2))
			{
				EndShot();
			}
		}
	}

	public void TakeShot()
	{
		ShotTaken = true;
	}

	private void EndShot()
	{
		GD.Print("Stopping all balls");
		CallDeferred("StopAllBalls");
		GD.PrintT(AllBalls);
		GD.Print("Shot Ended");
		EmitSignal(SignalName.ShotEnded, this);

		ShotTaken = false;
		
	}

	private void EndTurn()
	{

	}

	public void StopAllBalls()
	{
		Array.ForEach(AllBalls, b => b.Stop());
	}

	public static Vector2 GetStartPosition(BallType type)
	{
		return _startPositions[type];
	}
}


