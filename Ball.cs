using Godot;
using System;
using System.Linq;

public class Ball : RigidBody2D
{
	public override string ToString()	{
		return $"[{BallType} {Position}]";
    }
	
	[Signal]
	public delegate void SpeedChanged(float newSpeed);

	[Signal]
	public delegate void Carom(Ball ball);

	[Signal]
	public delegate void ShotTaken();

	[Export]
	public bool IsCueball { get; set; }

	public bool IsPocketed { get; set; }

	[Export]
	public BallType BallType { get; set; }

	public Vector2 StartPosition { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartPosition = Position;
		GD.Print($"BallType: {BallType}");		
	}

	public override void _Input(InputEvent @event)
	{
		if (Main.Started && IsCueball && @event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == 1 && eventMouseButton.IsPressed())
			{
				GD.Print("Shot Taken");
				EmitSignal("ShotTaken");
				LinearVelocity = new Vector2(eventMouseButton.Position - Position);//				
			}
		}
	}

	public override void _Process(float delta)
	{
		if (LinearVelocity == Vector2.Zero)		{
			return;
		}
		else if (LinearVelocity.Length() < 10)
		{
			LinearDamp = 0.5f;
		}
		else
		{
			LinearDamp = 0.25f;
		}
		//EmitSignal("SpeedChanged", LinearVelocity.Length());
	}

	public void OnCollision(object body)
	{
		if (body is Ball ball)
		{
			EmitSignal("Carom", ball);
		}
		else if (body is Pocket pocket)
		{
			GD.Print($"{this} pocketed in {pocket}");
			GD.Print($"{(int)BallType} points!");			
		}
		else
		{
			GD.Print($"{this} collided with {body}");
		}
		//GD.Print($"{this} Collided with {body}");
	}

	public void Stop()
	{
		GD.Print($"Stopping {this}");
		SetDeferred("linear_velocity", Vector2.Zero);		
	}

	internal void Reset()
	{
		Stop();
		GD.Print($"Setting {this} Position to {StartPosition}");
		Position = StartPosition;
		//SetDeferred("position", StartPosition);
        GD.Print($"{this} Position: {Position}");
        Visible = true;
		IsPocketed = false;
	}
}
