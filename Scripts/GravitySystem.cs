using System;
using System.Collections.Generic;

public class GravitySystem
{
    public LinkedList<GravityBody> gravityBodies;

    public GravitySystem(LinkedList<GravityBody> bodies)
    {
        gravityBodies = bodies;
    }

    private Vector[] CalculateAccelerations(int n)
    {
        int size = gravityBodies.Count;

        Vector[,] acceleration_matrix = new Vector[size, size];
        LinkedListNode<GravityBody> cur_a, cur_b;

        // Top right half of matrix (from a to b), without masses multiplied
        cur_a = gravityBodies.First;

        for (int a = 0; a < size; a++)
        {
            cur_b = cur_a.Next;
            for (int b = a + 1; b < size; b++)
            {
                Vector diff = cur_b.Value.GetPosition(n) - cur_a.Value.GetPosition(n); // Vector from a to b

                acceleration_matrix[a, b] = diff * (6.6743e-11d * Math.Pow(diff * diff, -1.5d));

                cur_b = cur_b.Next;
            }
            cur_a = cur_a.Next;
        }



        // Bottom left half of matrix (from b to a) by copying, multiplied by mass of a, inverted
        cur_a = gravityBodies.First;
        for (int a = 0; a < size; a++)
        {
            cur_b = cur_a.Next;
            for (int b = a + 1; b < size; b++)
            {
                acceleration_matrix[b, a] = acceleration_matrix[a, b] * -cur_a.Value.mass;

                cur_b = cur_b.Next;
            }
            cur_a = cur_a.Next;
        }

        // Top right half of matrix (from a to b), multiplied by mass of b
        cur_a = gravityBodies.First;
        for (int a = 0; a < size; a++)
        {
            cur_b = cur_a.Next;
            for (int b = a + 1; b < size; b++)
            {
                acceleration_matrix[a, b] = acceleration_matrix[a, b] * cur_b.Value.mass;
            }
            cur_a = cur_a.Next;
        }


        // sum up total acceleration
        Vector[] accelerations = new Vector[size];

        LinkedListNode<GravityBody> cur_body = gravityBodies.First;
        for (int i = 0; i < size; i++)
        {
            Vector sum = cur_body.Value.externalAcceleration;

            for (int j = 0; j < size; j++)
            {
                sum += acceleration_matrix[i, j];
            }

            accelerations[i] = sum;

            cur_body = cur_body.Next;
        }

        return accelerations;
    }

    public void Update(double dt)
    {
        int i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(1, gravityBody.GetPosition(0));
            gravityBody.SetVelocity(1, gravityBody.GetVelocity(0));

            i++;
        }

        Vector[] accelerations_k1 = CalculateAccelerations(1);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(2, gravityBody.GetPosition(0) + gravityBody.GetVelocity(1) * 0.5d * dt);
            gravityBody.SetVelocity(2, gravityBody.GetVelocity(0) + accelerations_k1[i] * 0.5d * dt);

            i++;
        }

        Vector[] accelerations_k2 = CalculateAccelerations(2);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(3, gravityBody.GetPosition(0) + gravityBody.GetVelocity(2) * 0.5d * dt);
            gravityBody.SetVelocity(3, gravityBody.GetVelocity(0) + accelerations_k2[i] * 0.5d * dt);

            i++;
        }

        Vector[] accelerations_k3 = CalculateAccelerations(3);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(4, gravityBody.GetPosition(0) + gravityBody.GetVelocity(3) * dt);
            gravityBody.SetVelocity(4, gravityBody.GetVelocity(0) + accelerations_k3[i] * dt);


            i++;
        }

        Vector[] accelerations_k4 = CalculateAccelerations(4);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(0, gravityBody.GetPosition(0) + (gravityBody.GetVelocity(1) + gravityBody.GetVelocity(2) * 2d + gravityBody.GetVelocity(3) * 2d + gravityBody.GetVelocity(4)) * (dt / 6d));
            gravityBody.SetVelocity(0, gravityBody.GetVelocity(0) + (accelerations_k1[i] + accelerations_k2[i] * 2d + accelerations_k3[i] * 2d + accelerations_k4[i]) * (dt / 6d));
            gravityBody.angle += gravityBody.angularVelocity;
            gravityBody.angularVelocity += gravityBody.externalAngularAcceleration;

            i++;
        }
    }
}
