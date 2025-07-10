using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
	private Camera2D camera;
	private double cameraScale = 1e-10d;
	private bool dragging = false;
	private Vector2 dragPos;
	private GravityBody pointOfInterest;
	private LinkedList<Planet> planets;
	private LinkedList<Rocket> rockets;
	private LinkedList<GravityBody> gravityBodies;
	private GravitySystem gravitySystem;
	private int simSpeedIndex;
	private long simSpeed;
	private Texture2D background;

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

		background = TryGetTexture("background.png");

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
		simSpeed = 1L;
		simSpeedIndex = 0;
		
		gravitySystem.CalculateSphereOfInfluences();

		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		for (int step = 0; step < 100; step++)
		{
			gravitySystem.Update(delta * simSpeed * 0.01d);
		}
		
		//camera.Position = (pointOfInterest.position * cameraScale).ToGodot();
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

		// Orbit line
		DrawPolyline(positions, new Color(255f, 255f, 255f, 0.25f), 2f);

		// Only draw sphere of influence if not binary primary or if secondary mass exceeds 10% of primary mass
		if (!body.isPrimary || body.binaryPartner.mass / body.mass > 0.1d)
		{
			if (body.a != 0d) // Don't draw root bodies sphere of influence
			{
				DrawCircle((body.position * cameraScale).ToGodot(), (float)(body.sphereOfInfluence * cameraScale), new Color(255f, 255f, 255f, 0.1f));
			}
		}
		
		if (body.isPrimary)
		{
			body.frameOfReference.position = (body.position * body.mass + body.binaryPartner.position * body.binaryPartner.mass) / (body.mass + body.binaryPartner.mass);
			//camera.Position = (body.frameOfReference.position * cameraScale).ToGodot();
			DrawOrbit(body.frameOfReference);
		}
	}

	public override void _Draw()
	{
		// Draw Background
		Vector2 screenSize = GetViewportRect().Size;
		Vector2 textureSize = background.GetSize();

		Vector2 scaledSize = textureSize * Mathf.Max(screenSize.X / textureSize.X, screenSize.Y / textureSize.Y);

		DrawTextureRect(background, new Rect2(camera.Position - scaledSize * 0.5f, scaledSize), false);

		// Calculate orbit lines
		gravitySystem.CalculateSphereOfInfluences();

		foreach (Planet planet in planets)
		{
			// Draw orbit line + sphere of influence
			DrawOrbit(planet);

			Vector radius = new(planet.radius, planet.radius);

			// Draw name
			DrawString(ThemeDB.FallbackFont, ((planet.position + radius) * cameraScale).ToGodot(), planet.name);
			// Draw planet
			DrawTextureRect(planet.texture, new Rect2(((planet.position - radius) * cameraScale).ToGodot(), (radius * (cameraScale * 2.0d)).ToGodot()), false);
		}

		DrawString(ThemeDB.FallbackFont, camera.Position - screenSize * 0.5f + new Vector2(0f, 16f), simSpeed + "x");
	}

	private static long LongPow(long x, long pow)
	{
		long ret = 1;
		while ( pow > 0 )
		{
			if ( (pow & 1) == 1 )
				ret *= x;
			x *= x;
			pow >>= 1;
		}
		return ret;
	}

	private void CalculateSimSpeed()
	{
		long[] simSpeeds = [1L, 2L, 5L];

		simSpeed = simSpeeds[simSpeedIndex % 3L] * LongPow(10L, simSpeedIndex / 3L);
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left)
			{
				if (mb.Pressed)
				{
					dragging = true;
					dragPos = mb.Position;
				}
				else
				{
					dragging = false;
				}
			}
			else if (mb.ButtonIndex == MouseButton.WheelUp)
			{
				cameraScale *= 1.1d;
				camera.Position *= 1.1f;
			}
			else if (mb.ButtonIndex == MouseButton.WheelDown)
			{
				cameraScale /= 1.1d;
				camera.Position /= 1.1f;
			}
		}
		else if (@event is InputEventMouseMotion mm)
		{
			if (dragging)
			{
				Vector2 delta = mm.Position - dragPos;
				camera.Position -= delta;
				dragPos = mm.Position;
			}
		}else if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			switch (keyEvent.Keycode)
			{
				case(Key.Plus):
					if(keyEvent.CtrlPressed)
					{
						simSpeedIndex += 1;
					}
					else
					{
						cameraScale *= 1.2d;
						camera.Position *= 1.2f;
					}
					break;
				case(Key.Minus):
					if(keyEvent.CtrlPressed)
					{
						simSpeedIndex -= 1;
						if (simSpeedIndex < 0)
						{
							simSpeedIndex = 0;
						}
					}
					else
					{
						cameraScale /= 1.2d;
						camera.Position /= 1.2f;
					}
				break;
			};

			CalculateSimSpeed();
		}
	}
}
