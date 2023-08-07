using Godot;
using System;

public partial class Cue : Node2D
{

	#region Lifecycle

	//How long it takes to charge up to full power
	const double MAX_CHARGE_TIME = 1.5;
	//How long it takes the cue to strike the ball from when the player releases the cue
	const double STRIKE_PERIOD = 0.1;

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
				ChargeShot(delta / MAX_CHARGE_TIME);
			}
			else if (Input.IsActionJustReleased("ChargeUpShot"))
			{
				_shotTaken = true;
				_remainingPower = _chargeStrength;
				return;
			}
			else if (!_isCharging && _isAiming) // Cannot drag once you are charging
			{
				Rotation = (GetGlobalMousePosition() - GlobalPosition).Angle() - Mathf.DegToRad(90);
			}
		}
		else if (_shotTaken)
		{
			DischargeShot(-delta / STRIKE_PERIOD);
		}
		CueTexture.Position = new Vector2(0, CalculateOffset());
	}

	private void ResetCue()
	{
		_chargeStrength = 0;
		_isCharging = false;
		_remainingPower = 0;
		_shotTaken = false;
	}

	#endregion

	#region Layout
	//This should be equal to the radius of the ball
	const int MIN_OFFSET = 12;
	Sprite2D CueTexture => GetNode<Sprite2D>("CueTexture");
	public void TargetCueBall(Vector2 cueballPosition)
	{
		Position = cueballPosition;
	}

	public float CalculateOffset()
	{
		if (!_shotTaken && _isCharging) 
		{
			return MIN_OFFSET + _chargeStrength;
		}
		else if (_shotTaken)
		{
			return MIN_OFFSET + _remainingPower;
		}
		return MIN_OFFSET;
	}

	#endregion

	#region Logic

	private bool _isAiming = false;
	private bool _isCharging = false;
	private bool _shotTaken = false;
	private float _remainingPower;

	private float _chargeStrength = 10;
	private int _maxChargeStrength = 100;

	private void ChargeShot(double percentCharge)
	{
		float increment = (float)(percentCharge * _maxChargeStrength);
		GD.Print($"Incrementing Charge by: {percentCharge}% or {increment}");
		_chargeStrength = Math.Min(_chargeStrength + increment, _maxChargeStrength);
	}

	private void DischargeShot(double percentCharge)
	{
		float decrement = (float)(percentCharge * _chargeStrength);
		GD.Print($"Decrementing Charge by: {percentCharge}% or {decrement}");
		_remainingPower = Math.Max(_remainingPower + decrement, 0);
		if (_remainingPower == 0)
		{
			EmitSignal(SignalName.Shoot, Rotation, _chargeStrength);
			ResetCue();
		}
	}

	#endregion




}
