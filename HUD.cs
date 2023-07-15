using Godot;
using System;

public partial class HUD : CanvasLayer
{

	[Signal]
	public delegate void StartGameEventHandler();
	
	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowGameOver()
	{
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, "timeout");

		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GetNode<Button>("StartButton").Show();
	}
	
	public void UpdateScore(float score)
	{
		//GetNode<Label>("ScoreLabel").Text = ((int)score).ToString();
	}
	
	public void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	public void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}
