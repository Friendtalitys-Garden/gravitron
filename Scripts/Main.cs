using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	private LinkedList<GravityBody> gravityBodies;

	private Texture2D gd;
	
	private Planet sun = new Planet()
	{
		mass = 1.989e30d,
		radius = 6.9634e8d
	};

	private Planet earth = new Planet()
	{
		mass = 5.972e24d,
		radius = 6.378e6d,

		position = new Vector(1.5205e11d, 0d),
		velocity = new Vector(0d, 29784.8d)
	};

	private GravitySystem gravitySystem;

	public override void _Ready()
	{
		gd = (Texture2D)GD.Load<Texture2D>("res://Sprites/icon.svg");

		gravityBodies = new LinkedList<GravityBody>();
		
		gravityBodies.AddLast(sun);
		gravityBodies.AddLast(earth);

		gravitySystem = new GravitySystem(gravityBodies);

		// Trigger initial draw
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		for (int step = 0; step < 100; step++)
		{
			bool st = earth.position.y < 0d;
			gravitySystem.Update(100d);
		}

		// Redraw every frame
		QueueRedraw();
	}

	public override void _Draw()
	{
		Color color = new Color(1, 0, 0); // Red
		
		DrawTexture(gd, (sun.position * 3e-9d).ToGodot() - gd.GetSize() * 0.5f);
		DrawTexture(gd, (earth.position * 3e-9d).ToGodot() - gd.GetSize() * 0.5f);

		// Orbit lines
		int size = gravityBodies.Count;
		Vector[,] positions = new Vector[size, 100];
		int i;

		foreach (GravityBody body in gravityBodies)
		{
			body.position_save = body.position;
			body.velocity_save = body.velocity;
		}

		for (int step = 0; step < 100; step++)
		{
			gravitySystem.Update(100000d);

			i = 0;
			foreach (GravityBody body in gravityBodies)
			{
				positions[i, step] = body.position;

				i++;
			}
		}

		foreach (GravityBody body in gravityBodies)
		{
			body.position = body.position_save;
			body.velocity = body.velocity_save;
		}

		Vector2[] polyLine = new Vector2[100];


		for (int step = 0; step < 100; step++)
		{
			polyLine[step] = (positions[1, step] * 3e-9d).ToGodot();
		}

		DrawPolyline(polyLine, Colors.White, 5f);
	}
}
