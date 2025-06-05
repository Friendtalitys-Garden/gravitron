public class Vector
{
    public double x, y;

    public Vector(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.x + b.x, a.x + b.x);
    }

    public static Vector operator *(Vector v, double d)
    {
        return new Vector(v.x * d, v.y * d);
    }
}