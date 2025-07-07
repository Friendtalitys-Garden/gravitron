using Godot;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;

public partial class Main : Node2D
{
	private Camera2D camera;
	private GravityBody pointOfInterest;
	private LinkedList<Planet> planets;
	private LinkedList<Rocket> rockets;
	private LinkedList<GravityBody> gravityBodies;
	private GravitySystem gravitySystem;

	public override void _Ready()
	{
		camera = new Camera2D();
		AddChild(camera);
		camera.MakeCurrent();

		DatabaseConnection db = new();
		db.Setup();

		gravityBodies = new LinkedList<GravityBody>();

		planets = db.ReadPlanets();
		foreach (var planet in planets) gravityBodies.AddLast(planet);

		rockets = db.ReadRockets();
		foreach (var rocket in rockets) gravityBodies.AddLast(rocket);

		gravitySystem = new GravitySystem(gravityBodies);
		pointOfInterest = planets.FirstOrDefault(p => p.name == "Sun");

		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		for (int step = 0; step < 100; step++)
		{
			gravitySystem.Update(100d);
		}

		
		camera.Position = (pointOfInterest.position * 3e-9d).ToGodot();
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

		for (i = 0; i < size; i++)
		{
			Vector2[] polyLine = new Vector2[100];

			for (int step = 0; step < 100; step++)
			{
				polyLine[step] = (positions[i, step] * 3e-9d).ToGodot();
			}

			DrawPolyline(polyLine, Colors.White, 2f);
		}

		// Planets
		//DrawTextureRect(sunp, new Rect2((sun.position * 3e-9d).ToGodot() - new Vector2(100f, 100f) * 0.5f, new Vector2(100f, 100f)), false);
		//DrawTextureRect(earthp, new Rect2((earth.position * 3e-9d).ToGodot() - new Vector2(50f, 50f) * 0.5f, new Vector2(50f, 50f)), false);
	}
}
