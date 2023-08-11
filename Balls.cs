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

	public Ball Cueball => AllBalls.SingleOrDefault(b => b.IsCueball);

	static Dictionary<BallType, Vector2> _startPositions;

	[Signal]
	public delegate void ShotTakenEventHandler();
	
	[Signal]
	public delegate void ShotEndedEventHandler(Balls balls);

	public Ball[] AllBalls => GetChildren().OfType<Ball>().ToArray();

	public static int BallIdCounter;
	bool _shotTaken;
	
	public override void _Ready()
	{
		//Debugger.Launch();
		_startPositions = AllBalls.ToDictionary(d => d.BallType, r => r.Position);
		//Cueball = AllBalls.Single(b => b.IsCueball);

	}

	public override void _Process(double delta)
	{
		if (_shotTaken)
		{
			if (AllBalls.Any() && AllBalls.All(b => b.LinearVelocity.Length() < 2))
			{
				EndShot();
			}
		}
	}

	public void TakeShot()
	{
		_shotTaken = true;
	}

	public void StrikeCueBall(Vector2 velocity)
	{
		var cueball = AllBalls.Single(f => f.IsCueball);
		cueball.LinearVelocity = velocity;
		TakeShot();
		EmitSignal(SignalName.ShotTaken);
		
	}

	private void EndShot()
	{
		GD.Print("Stopping all balls");
		CallDeferred("StopAllBalls");
		GD.PrintT(AllBalls);
		GD.Print("Shot Ended");
		EmitSignal(SignalName.ShotEnded, this);
		_shotTaken = false;		
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


