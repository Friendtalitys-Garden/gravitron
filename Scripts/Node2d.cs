using Godot;
using System;

public partial class Node2d : Node2D
{
	private Vector2 _circlePosition = new Vector2(0, 150); // Start position
	private float _speed = 100f; // Pixels per second

	public override void _Ready()
	{
		// Trigger initial draw
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		// Move circle to the right
		_circlePosition.X += (float)(delta * _speed);

		// Redraw every frame
		QueueRedraw();
	}

	public override void _Draw()
	{
		float radius = 30;
		Color color = new Color(1, 0, 0); // Red

		DrawCircle(_circlePosition, radius, color);
	}
}
