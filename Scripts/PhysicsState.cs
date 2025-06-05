public class PhysicsState
{
    public Vector position, velocity;

    public PhysicsState(Vector position, Vector velocity)
    {
        this.position = position;
        this.velocity = velocity;
    }

    public static PhysicsState operator +(PhysicsState a, PhysicsState b)
    {
        return new PhysicsState(a.position + b.position, a.velocity + b.velocity);
    }

    public static PhysicsState operator *(PhysicsState v, double d)
    {
        return new PhysicsState(v.position * d, v.velocity * d);
    }

    public PhysicsState derivative(Vector acceleration)
    {
        return new PhysicsState(velocity, acceleration);
    }
}