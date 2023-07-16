using Godot;
using System;
using System.Linq;

public partial class Ball : RigidBody2D
{
	public override string ToString()	{
		return $"[{BallType} {Position}]";
	}

	[Signal]
	public delegate void SpeedChangedEventHandler(float newSpeed);

	[Signal]
	public delegate void CaromEventHandler(Ball ball);

	[Signal]
	public delegate void ShotTakenEventHandler();

	[Export]
	public bool IsCueball { get; set; }

	public bool IsPocketed { get; set; }

	[Export]
	public BallType BallType { get; set; }

	public Vector2 StartPosition { get; set; }

	public Sprite2D Sprite { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartPosition = Position;
		GD.Print($"BallType: {BallType}");
		Sprite = GetChildren().OfType<Sprite2D>().First();
	}

	public override void _Input(InputEvent @event)
	{
		if (Main.Started && IsCueball && @event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.IsPressed())
			{
				GD.Print("Shot Taken");
				EmitSignal(SignalName.ShotTaken);
				LinearVelocity = eventMouseButton.Position - Position;				
			}
		}
	}
	
	public override void _Process(double delta)
	{
		if (LinearVelocity == Vector2.Zero)	
		{
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

	public void OnCollision(Node body)
	{
		if (body is Ball ball)
		{
			EmitSignal(SignalName.Carom, ball);
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

	public void Clone(Ball otherBall)
	{
		IsCueball = otherBall.IsCueball;
		BallType = otherBall.BallType;
		GetNode<Sprite2D>("Sprite2D").Texture = otherBall.GetNode<Sprite2D>("Sprite2D").Texture;
	}

	public void Stop()
	{
		GD.Print($"Stopping {this}");
		SetDeferred("linear_velocity", Vector2.Zero);		
	}

	internal void Reset()
	{
		//Stop();
		GD.Print($"Setting {this} Position to {StartPosition}");
		//Mode = ModeEnum.Static;
		Position = StartPosition;
		//Mode = ModeEnum.Rigid;
		SetDeferred("position", StartPosition);
		GD.Print($"{this} Position: {Position}");
		Visible = true;
		IsPocketed = false;
	}

	private void _on_body_entered(Node body)
	{
		// Replace with function body.
	}
}

