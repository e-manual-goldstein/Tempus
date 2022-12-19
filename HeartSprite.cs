using Godot;
using System;

public class HeartSprite : Sprite
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private int Speed = 400;
	private float AngularSpeed = Mathf.Pi;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Love!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var direction = 0;
		if (Input.IsActionPressed("ui_left"))
		{
			direction = -1;
		}
		if (Input.IsActionPressed("ui_right"))
		{
			direction = 1;
		}

		Rotation += AngularSpeed * direction * delta;

		var velocity = Vector2.Zero;
		if (Input.IsActionPressed("ui_up"))
		{
			velocity = Vector2.Up.Rotated(Rotation) * Speed;
		}

		Position += velocity * delta;
	}
	
	private void Orbit(float delta)
	{
		var velocity = Vector2.Up.Rotated(Rotation) * Speed;

		Position += velocity * delta;
		Rotation += AngularSpeed * delta;
	}
}
