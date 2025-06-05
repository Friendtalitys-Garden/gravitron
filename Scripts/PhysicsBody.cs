using Godot

public abstract class PhysicsBody
{
    public Vector position;
    public Vector velocity;

    public void update(Vector acceleration, double dt)
    {
        PhysicsState ps = new PhysicsState(position, velocity);

        PhysicsState k1 = ps.derivative(acceleration);
        PhysicsState k2 = (ps + k1 * 0.5d * dt).derivative(acceleration);
        PhysicsState k3 = (ps + k2 * 0.5d * dt).derivative(acceleration);
        PhysicsState k4 = (ps + k3 * dt).derivative(acceleration);

        ps = ps + (k1 + k2 * 2d + k3 * 2d + k4) * dt / 6d;

        position = ps.position;
        velocity = ps.velocity;
    }
}