using Godot;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;

public partial class Main : Node2D
{
	private Camera2D camera;
	private double cameraScale = 1e-10d;
	private GravityBody pointOfInterest;
	private LinkedList<Planet> planets;
	private LinkedList<Rocket> rockets;
	private LinkedList<GravityBody> gravityBodies;
	private GravitySystem gravitySystem;

	private static Texture2D TryGetTexture(string name)
	{
		Texture2D texture = GD.Load<Texture2D>("res://Sprites/" + name);
		// "Compund Assignment"
		texture ??= GD.Load<Texture2D>("res://Sprites/icon.svg");

		return texture;
	}

	public override void _Ready()
	{
		camera = new Camera2D();
		AddChild(camera);
		camera.MakeCurrent();

		DatabaseConnection db = new();
		db.Setup();

		gravityBodies = new LinkedList<GravityBody>();

		planets = db.ReadPlanets();
		foreach (var planet in planets)
		{
			planet.texture = TryGetTexture(planet.sprite);
			gravityBodies.AddLast(planet);
		}

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
			gravitySystem.Update(1000d);
		}
		
		camera.Position = (pointOfInterest.position * cameraScale).ToGodot();
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
			i = 0;
			foreach (GravityBody body in gravityBodies)
			{
				positions[i, step] = body.position;

				i++;
			}
			
			gravitySystem.Update(100000d);
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
				polyLine[step] = (positions[i, step] * cameraScale).ToGodot();
			}

			DrawPolyline(polyLine, Colors.White, 2f);
		}

		foreach (Planet planet in planets)
		{
			Vector radius = new(planet.radius, planet.radius);

			Font defaultFont = ThemeDB.FallbackFont;
			DrawString(defaultFont, ((planet.position + radius) * cameraScale).ToGodot(), planet.name);
			DrawTextureRect(planet.texture, new Rect2(((planet.position - radius) * cameraScale).ToGodot(), (radius * (cameraScale * 2.0d)).ToGodot()), false);
		}
	}
}
