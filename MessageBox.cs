using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class MessageBox : VBoxContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debugger.Launch();
		MessageReceived += ShowMessage;
		Parent = GetParent<ScrollContainer>();
		
	}
	ScrollContainer Parent { get; set; }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var scrollbar = Parent.GetVScrollBar();
		Parent.ScrollVertical = (int)scrollbar.MaxValue;
	}

	public void ShowMessage(string message)
	{
		var label = new Label() { Text = message };
		label.Size = new Vector2(Size.X, 20);
		AddChild(label);
	}

	private static event Action<string> MessageReceived;

	public static void PrintMessage(string message)
	{
		MessageReceived(message);
	}
}
