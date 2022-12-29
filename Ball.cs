using Godot;
using System;
using System.Linq;

public class Ball : RigidBody2D
{
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
		if (IsCueball && @event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == 1 && eventMouseButton.IsPressed())
			{
				LinearVelocity = new Vector2(eventMouseButton.Position - Position);
				GD.Print($"Cueball Position = {Position}");
				GD.Print($"Click Position = {eventMouseButton.Position}");
				GD.Print($"Linear Velocity = {LinearVelocity}");
				GD.Print($"Linear Speed = {LinearVelocity.Length()}");
				GD.Print($"Linear Speed Normalised = {LinearVelocity.Normalized()}");
				//LinearVelocity.
			}
		}
	}
	
	public override string ToString()
	{
		return $"{BallType}";
	}

	public void OnCollision(object body)
	{
		GD.Print($"{this} Collided with {body}");
	}
}
