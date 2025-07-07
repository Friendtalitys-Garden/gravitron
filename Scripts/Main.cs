using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	private Camera2D camera;
	private LinkedList<GravityBody> gravityBodies;


	private Texture2D sunp;
	private Texture2D earthp;

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
		camera = new Camera2D();
		AddChild(camera);
		camera.MakeCurrent();
		camera.Position = (earth.position * 3e-9d).ToGodot();

		sunp = (Texture2D)GD.Load<Texture2D>("res://Sprites/sun.png");
		earthp = (Texture2D)GD.Load<Texture2D>("res://Sprites/earth.png");

		gravityBodies = new LinkedList<GravityBody>();

		gravityBodies.AddLast(sun);
		gravityBodies.AddLast(earth);

		gravitySystem = new GravitySystem(gravityBodies);

		// Trigger initial draw
		QueueRedraw();

		DatabaseConnection db = new();
		db.Setup();
		db.CreateTables();
	}

	public override void _Process(double delta)
	{
		for (int step = 0; step < 100; step++)
		{
			gravitySystem.Update(100d);
		}

		// Redraw every frame

		camera.Position = (sun.position * 3e-9d).ToGodot();
		QueueRedraw();
	}

	public override void _Draw()
	{
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

		// Planets
		DrawTextureRect(sunp, new Rect2((sun.position * 3e-9d).ToGodot() - new Vector2(100f, 100f) * 0.5f, new Vector2(100f, 100f)), false);
		DrawTextureRect(earthp, new Rect2((earth.position * 3e-9d).ToGodot() - new Vector2(50f, 50f) * 0.5f, new Vector2(50f, 50f)), false);
	}
}
