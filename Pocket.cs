using Godot;
using GodotBilliards;
using System;

public class Pocket : Area2D
{
	[Signal]
	public delegate void BallPocketed(Ball ball);

	[Export]
	public PocketLocation PocketLocation { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	private void ShapeEntered(RID body_rid, object body, int body_shape_index, int local_shape_index)
	{
		// Replace with function body.
		GD.Print($"{body} entered {this}");
		if (body is Ball ball)
		{
			EmitSignal("BallPocketed", ball);
		}
	}

	public override string ToString()
	{
		return $"{PocketLocation}";
	}
}

