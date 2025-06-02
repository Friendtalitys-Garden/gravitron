using Godot;
using System;

public partial class Node2d : Node2D
{
	 public override void _Ready()
	{
		// This ensures the node is redrawn when ready

	}

	public override void _Draw()
	{
		DrawCircle(new Vector2(200, 150), 50, new Color(1, 0, 0));
	}
}
