using Godot;
using GodotBilliards;
using System;

public partial class Pocket : Area2D
{
	[Signal]
	public delegate void BallPocketedEventHandler(Ball ball);

	[Export]
	public PocketLocation PocketLocation { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void ShapeEntered(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
	{
		// Replace with function body.
		GD.Print($"{body} entered {this}");
		if (body is Ball ball)
		{
			EmitSignal(SignalName.BallPocketed, ball);
		}
	}

	private void _on_body_shape_entered(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
	{
		// Replace with function body.
		GD.Print($"{body} entered {this}");
		if (body is Ball ball)
		{
			EmitSignal(SignalName.BallPocketed, ball);
		}
	}

	public override string ToString()
	{
		return $"{PocketLocation}";
	}
}


