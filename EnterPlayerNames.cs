using Godot;
using System;
using System.Linq;

public partial class EnterPlayerNames : Node2D
{
	public Action<string[]> StartButtonPressed { get; internal set; }

	Button StartButton => GetNode<Button>("Panel/Button");
	TextEdit[] NameFields => GetNode<Panel>("Panel").GetChildren().OfType<TextEdit>().ToArray();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartButton.Pressed += StartButton_Pressed;
	}

	private void StartButton_Pressed()
	{
		var names = NameFields.Where(f => !string.IsNullOrEmpty(f.Text)).Select(r => r.Text).ToArray();
		StartButtonPressed(names);
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
