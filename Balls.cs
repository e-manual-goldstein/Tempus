using Godot;
using System;
using System.Linq;

public partial class Balls : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	[Signal]
	public delegate void ShotEndedEventHandler();

	public Ball[] AllBalls { get; set; }
	
	public bool ShotTaken { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AllBalls = GetChildren().OfType<Ball>().ToArray();
		
	}

	public override void _Process(double delta)
	{
		if (ShotTaken)
		{
			if (AllBalls.Any() && AllBalls.All(b => b.LinearVelocity.Length() < 2))
			{
				GD.Print("Stopping all balls");
				CallDeferred("StopAllBalls");
				GD.PrintT(AllBalls);
				GD.Print("Shot Ended");
				EmitSignal(SignalName.ShotEnded);
				
				ShotTaken = false;
			}
		}
	}

	public void TakeShot()
	{
		ShotTaken = true;
	}

	public void StopAllBalls()
	{
		Array.ForEach(AllBalls, b => b.Stop());
	}
}


