using Godot;
using System;

public class HeartSprite : Sprite
{

	[Signal]
	delegate void MySignal();

	[Signal]
	delegate void MySignalWithArguments(string foo, int bar);
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private int Speed = 400;
	private float AngularSpeed = Mathf.Pi;
	private bool _orbit = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Love!");
		var timer = GetNode("Timer");
		var error = timer.Connect("timeout", this, "_on_Timer_timeout");
		if (error != Error.Ok)
		{
			GD.Print("Oops");
			GD.Print(error);
		}
	}
	
	private void _on_Timer_timeout()
	{
		Visible = !Visible;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (_orbit)
		{
			Orbit(delta);
		}
		
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
	
	public void _on_Button_pressed()
	{
		_orbit = !_orbit;
		EmitSignal("MySignal");
		EmitSignal("MySignal", "STRING", 69);
	}
}



