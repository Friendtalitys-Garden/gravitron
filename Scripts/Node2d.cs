using Godot;

public partial class Node2d : Node2D
{
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

	private GravitySystem gravitySystem = new GravitySystem();

	public override void _Ready()
	{
		gravitySystem.gravityBodies.AddLast(sun);
		gravitySystem.gravityBodies.AddLast(earth);

		// Trigger initial draw
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < 100; i++)
		{
			bool st = earth.position.y < 0d;
			gravitySystem.Update(100d);
			if (st & earth.position.y > 0d)
			{
				GD.Print(earth.position.x);
			}
		}

		// Redraw every frame
		QueueRedraw();
	}

	public override void _Draw()
	{
		Color color = new Color(1, 0, 0); // Red

		DrawCircle((sun.position * 3e-9d).ToGodot(), 10f, color);
		DrawCircle((earth.position * 3e-9d).ToGodot(), 10f, color);
	}
}
