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
		string path = "res://Sprites/" + name;

		Texture2D texture;
		if (ResourceLoader.Exists(path))
		{
			texture = ResourceLoader.Load<Texture2D>(path);
		}
		else
		{
			texture = ResourceLoader.Load<Texture2D>("res://Sprites/icon.svg");
		}
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
		
		gravitySystem.CalculateSphereOfInfluences();

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

	public void DrawOrbit(GravityBody body)
	{
		int halfResolution = 500;

		Vector2[] positions = new Vector2[2 * halfResolution + 1];

		double limit = Math.PI;
		if (body.e > 1)
		{
			limit = Math.Acos(-1d / body.e);
		}

		for (int i = 0; i <= 2 * halfResolution; i++)
		{
			double trueAnomaly = limit * ((double)(i - halfResolution) / halfResolution);

			double distance = body.a * (1d - body.e * body.e) / (1d + body.e * Math.Cos(trueAnomaly));

			positions[i] = ((body.frameOfReference.position + new Vector(distance * Math.Cos(trueAnomaly + body.p), distance * Math.Sin(trueAnomaly + body.p))) * cameraScale).ToGodot();
		}

		DrawPolyline(positions, Colors.White, 2f);

		if (body.isPrimary)
		{
			body.frameOfReference.position = (body.position * body.mass + body.binaryPartner.position * body.binaryPartner.mass) / (body.mass + body.binaryPartner.mass);
			//camera.Position = (body.frameOfReference.position * cameraScale).ToGodot();
			DrawOrbit(body.frameOfReference);
		}
	}

	public override void _Draw()
	{
		gravitySystem.CalculateSphereOfInfluences();

		foreach (Planet planet in planets)
		{
			DrawOrbit(planet);

			Vector radius = new(planet.radius, planet.radius);

			DrawString(ThemeDB.FallbackFont, ((planet.position + radius) * cameraScale).ToGodot(), planet.name);
			DrawTextureRect(planet.texture, new Rect2(((planet.position - radius) * cameraScale).ToGodot(), (radius * (cameraScale * 2.0d)).ToGodot()), false);
		}
	}
	
	
}
