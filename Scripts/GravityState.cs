using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class GravityState
{
    public Vector[] positions, velocities;
    public double[] angles, angularVelocities;

    public GravityState(LinkedList<GravityBody> gravityBodies)
    {
        positions = gravityBodies.Select(gravityBody => gravityBody.position).ToArray();
        velocities = gravityBodies.Select(gravityBody => gravityBody.velocity).ToArray();
        angles = gravityBodies.Select(gravityBody => gravityBody.angle).ToArray();
        angularVelocities = gravityBodies.Select(gravityBody => gravityBody.angularVelocity).ToArray();
    }

    public GravityState(Vector[] positions, Vector[] velocities, double[] angles, double[] angularVelocities)
    {
        this.positions = positions;
        this.velocities = velocities;
        this.angles = angles;
        this.angularVelocities = angularVelocities;
    }

    public static GravityState operator +(GravityState a, GravityState b)
    {
        return new GravityState(
            a.positions.Zip(b.positions, (x, y) => x + y).ToArray(),
            a.velocities.Zip(b.velocities, (x, y) => x + y).ToArray(),
            a.angles.Zip(b.angles, (x, y) => x + y).ToArray(),
            a.angularVelocities.Zip(b.angularVelocities, (x, y) => x + y).ToArray()
        );
    }

    public static GravityState operator *(GravityState v, double d)
    {
        return new GravityState(
            v.positions.Select(position => position * d).ToArray(),
            v.velocities.Select(velocity => velocity * d).ToArray(),
            v.angles.Select(angle => angle * d).ToArray(),
            v.angularVelocities.Select(angularVelocity => angularVelocity * d).ToArray()
        );
    }

    public GravityState derivative(Vector[] accelerations, double[] angularAccelerations)
    {
        return new GravityState(
            velocities,
            accelerations,
            angularVelocities,
            angularAccelerations
        );
    }
}