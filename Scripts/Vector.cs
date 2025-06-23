using Godot;

public struct Vector
{
	public double x, y;

	public static readonly Vector zero = new Vector(0d, 0d);

	public Vector(double x, double y)
	{
		this.x = x;
		this.y = y;
	}

	public readonly Vector2 ToGodot()
	{
		return new Vector2((float)x, (float)y);
	}

	public static Vector operator +(Vector a, Vector b)
	{
		return new Vector(a.x + b.x, a.y + b.y);
	}

	public static Vector operator -(Vector a, Vector b)
	{
		return new Vector(a.x - b.x, a.y - b.y);
	}

	public static Vector operator *(Vector v, double d)
	{
		return new Vector(v.x * d, v.y * d);
	}

	public static double operator *(Vector a, Vector b)
	{
		return a.x * b.x + a.y * b.y;
	}
}
