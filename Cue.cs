using Godot;
using System;

public partial class Cue : Node2D
{
	//This should be equal to the radius of the ball
	const int MIN_OFFSET = 12;
	//How long it takes to charge up to full power
	const double MAX_CHARGE_TIME = 1.5;
	//How long it takes the cue to strike the ball from when the player releases the cue
	const double STRIKE_PERIOD = 0.1;

	private bool _isAiming = false;
	private bool _isCharging = false;
	private bool _shotTaken = false;

	private Vector2 _initialPosition;
	private float _chargeStrength = 10;
	private int _maxChargeStrength = 100;
	Sprite2D CueTexture => GetNode<Sprite2D>("CueTexture");

	[Signal]
	public delegate void ShootEventHandler(float angle, float chargeStrength);

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
		if (!_shotTaken)
		{
			if (Input.IsActionPressed("ChargeUpShot"))
			{
				_isCharging = true;
				ChargeShotByXPercent(delta / MAX_CHARGE_TIME);			
			}
			else if (Input.IsActionJustReleased("ChargeUpShot"))
			{
				_shotTaken = true;
				EmitSignal(SignalName.Shoot, Rotation, _chargeStrength);
				return;
			}
			else if (!_isCharging && _isAiming) // Cannot drag once you are charging
			{
				Rotation = (GetGlobalMousePosition() - GlobalPosition).Angle() - Mathf.DegToRad(90);
			}
		}
		else if (_shotTaken)
		{
			ChargeShotByXPercent(-delta / STRIKE_PERIOD);
		}
		CueTexture.Position = new Vector2(0, MIN_OFFSET + (int)_chargeStrength);
	}

	private void ChargeShotByXPercent(double xPercent)
	{
		float increment = (float)(xPercent * _maxChargeStrength);
		GD.Print($"Incrementing Charge by: {xPercent}% or {increment}");
		_chargeStrength = Math.Min(_chargeStrength + increment, _maxChargeStrength); 
	}

	private void TargetCueball()
	{
		CueTexture.Position = new Vector2(0, MIN_OFFSET + (int)_chargeStrength);
		Rotation = Random.Shared.Next();
	}
}
