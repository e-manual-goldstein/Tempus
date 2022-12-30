using Godot;
using System;
using System.Linq;

public class Ball : RigidBody2D
{
	[Signal]
	public delegate void SpeedChanged(float newSpeed);

	[Signal]
	public delegate void Caram(Ball ball);

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
		var texture = GD.Load<Texture>($"res://assets/ball{BallType}_10.png");
		var sprite = GetChildren().OfType<Sprite>().Single();
		sprite.Texture = texture;		
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
		if (LinearVelocity.Length() < 10)
		{
			LinearDamp = 0.5f;
		}
		else
		{
			LinearDamp = 0.25f;
		}
		//EmitSignal("SpeedChanged", LinearVelocity.Length());
	}

	public override string ToString()
	{
		return $"{BallType}";
	}

	public void OnCollision(object body)
	{
		if (body is Ball ball)
		{
			EmitSignal("Caram", ball);
		}
		else if (body is Pocket pocket)
		{
			GD.Print($"{this} pocketed in {pocket}");
			GD.Print($"{(int)BallType} points!");			
		}
		else
		{
			//GD.Print($"{this} collided with {body}");
		}
		//GD.Print($"{this} Collided with {body}");
	}

	public void Stop()
	{
		LinearVelocity = Vector2.Zero;
	}

	internal void Reset()
	{
		SetDeferred("Position", StartPosition);
		Visible = true;
		IsPocketed = false;
	}
}
