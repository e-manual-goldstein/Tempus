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

	[Export]
	public BallType BallType { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
				LinearVelocity = new Vector2(eventMouseButton.Position - Position);
//				GD.Print($"Cueball Position = {Position}");
//				GD.Print($"Click Position = {eventMouseButton.Position}");
//				GD.Print($"Linear Velocity = {LinearVelocity}");
//				GD.Print($"Linear Speed = {LinearVelocity.Length()}");
//				GD.Print($"Linear Speed Normalised = {LinearVelocity.Normalized()}");
				//LinearVelocity.
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
		//GD.Print($"{this} Collided with {body}");
	}

	public void Stop()
	{
		LinearVelocity = new Vector2(0,0);
	}
}
