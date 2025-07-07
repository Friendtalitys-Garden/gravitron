using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	private Camera2D camera;
	private LinkedList<GravityBody> gravityBodies;


	private Texture2D sunp;
	private Texture2D earthp;
	private Texture2D marsp;

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

	private Planet mars = new Planet()
	{
		mass = 6.39e23d,
		radius = 3.3895e6d,

		position = new Vector(2.0662e11d, 0d),
		velocity = new Vector(0d, 26500d)
	};
	
	private Planet asteroid = new Planet()
	{
		mass = 6.39e29d,
		radius = 5000d,

		position = new Vector(1.2e11d, 1e11d),
		velocity = new Vector(0d, 16000d)
	};

	private GravitySystem gravitySystem;

	public override void _Ready()
	{
		camera = new Camera2D();
		AddChild(camera);
		camera.MakeCurrent();

		sunp = (Texture2D)GD.Load<Texture2D>("res://Sprites/sun.png");
		marsp = (Texture2D)GD.Load<Texture2D>("res://Sprites/mars.png");
		earthp = (Texture2D)GD.Load<Texture2D>("res://Sprites/earth.png");

		gravityBodies = new LinkedList<GravityBody>();
		
		gravityBodies.AddLast(sun);		
		gravityBodies.AddLast(mars);
		gravityBodies.AddLast(earth);
		gravityBodies.AddLast(asteroid);

		gravitySystem = new GravitySystem(gravityBodies);

		// Trigger initial draw
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		for (int step = 0; step < 200; step++)
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
		Vector[,] positions = new Vector[size, 2000];
		int i;

		foreach (GravityBody body in gravityBodies)
		{
			body.position_save = body.position;
			body.velocity_save = body.velocity;
		}

		for (int step = 0; step < 2000; step++)
		{
			gravitySystem.Update(10000d);

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

		Vector2[] polyLine1 = new Vector2[2000];
		Vector2[] polyLine2 = new Vector2[2000];
		Vector2[] polyLine3 = new Vector2[2000];



		for (int step = 0; step < 2000; step++)
		{
			polyLine1[step] = (positions[1, step] * 3e-9d).ToGodot();
			polyLine2[step] = (positions[2, step] * 3e-9d).ToGodot();
			polyLine3[step] = (positions[3, step] * 3e-9d).ToGodot();
		}

		DrawPolyline(polyLine1, Colors.White, 5f);
		DrawPolyline(polyLine2, Colors.White, 5f);
		DrawPolyline(polyLine3, Colors.White, 5f);
	
	// Planets
		DrawTextureRect(sunp, new Rect2((sun.position * 3e-9d).ToGodot() - new Vector2(100f, 100f) * 0.5f, new Vector2(100f, 100f)), false);
		DrawTextureRect(earthp, new Rect2((earth.position * 3e-9d).ToGodot() - new Vector2(50f, 50f) * 0.5f, new Vector2(50f, 50f)), false);
		DrawTextureRect(marsp, new Rect2((mars.position * 3e-9d).ToGodot() - new Vector2(50f, 50f) * 0.5f, new Vector2(50f, 50f)), false);
		DrawTextureRect(sunp, new Rect2((asteroid.position * 3e-9d).ToGodot() - new Vector2(50f, 50f) * 0.5f, new Vector2(50f, 50f)), false);

	}
}
