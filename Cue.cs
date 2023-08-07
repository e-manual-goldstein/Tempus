using Godot;
using System;

public partial class Cue : Node2D
{
	//This should be equal to the radius of the ball
	const int MIN_OFFSET = 12;

	const double MAX_CHARGE_TIME = 1.5;
	private bool _isAiming = false;
	private bool _isCharging = false;
	private Vector2 _initialPosition;
	private double _chargeStrength;
	private int _maxChargeStrength = 100;
	Sprite2D CueTexture => GetNode<Sprite2D>("CueTexture");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (!_isCharging && @event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == MouseButton.Left)
		{
			if (eventMouseButton.Pressed)
			{
				// Check if the left mouse button is pressed down
				_isAiming = true;				
			}
			else
			{
				// Left mouse button released, stop dragging
				_isAiming = false;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ChargeUpShot"))
		{
			_isCharging = true;
			ChargeShotByXPercent(delta / MAX_CHARGE_TIME);
			
		}
		else if (Input.IsActionJustReleased("ChargeUpShot"))
		{
			_isCharging = false;
			_chargeStrength = 0;
		}
		else if (!_isCharging && _isAiming) // Cannot drag once you are charging
		{
			Rotation = (GetGlobalMousePosition() - GlobalPosition).Angle() - Mathf.DegToRad(90);
		}
		CueTexture.Position = new Vector2(0, MIN_OFFSET + (int)_chargeStrength);
	}

	private void ChargeShotByXPercent(double xPercent)
	{
		var increment = xPercent * _maxChargeStrength;
		GD.Print($"Incrementing Charge by: {xPercent}% or {increment}");
		_chargeStrength = Math.Min(_chargeStrength + increment, _maxChargeStrength); 
	}

	private void TargetCueball()
	{
		CueTexture.Position = new Vector2(0, MIN_OFFSET + (int)_chargeStrength);
		Rotation = Random.Shared.Next();
	}
	
	private void Shoot(double power)
	{

	}

	private void Aim(float radians)
	{

	}
}
