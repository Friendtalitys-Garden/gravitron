using System;
using System.Collections.Generic;
using System.Linq;

public class GravitySystem
{
    public LinkedList<GravityBody> gravityBodies;

    private Vector[] CalculateAccelerations(int i)
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
                Vector diff = cur_b.Value.GetPosition(i) - cur_a.Value.GetPosition(i); // Vector from a to b

                acceleration_matrix[a, b] = diff * 0.000000000066743d * Math.Pow(-1.5, diff * diff);
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

    private double[] GetAngularAccelerations()
    {
        return gravityBodies.Select(gravityBody => gravityBody.externalAngularAcceleration).ToArray();
    }

    public void Update(double dt)
    {
        Vector[] accelerations = CalculateAccelerations(0);

        int i = 0;

        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(1, gravityBody.GetVelocity(0));
            gravityBody.SetVelocity(1, accelerations[i]);

            i ++;
        }

        Vector[] accelerations_k1 = CalculateAccelerations(1);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(2, gravityBody.GetVelocity(0) + gravityBody.GetVelocity(1) * 0.5d * dt);
            gravityBody.SetVelocity(2, accelerations[i] + accelerations_k1[i] * 0.5d * dt);

            i ++;
        }

        Vector[] accelerations_k2 = CalculateAccelerations(2);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(3, gravityBody.GetVelocity(0) + gravityBody.GetVelocity(2) * 0.5d * dt);
            gravityBody.SetVelocity(3, accelerations[i] + accelerations_k2[i] * 0.5d * dt);

            i ++;
        }

        Vector[] accelerations_k3 = CalculateAccelerations(3);
        i = 0;
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(4, gravityBody.GetVelocity(0) + gravityBody.GetVelocity(3) * dt);
            gravityBody.SetVelocity(4, accelerations[i] + accelerations_k3[i] * dt);


            i ++;
        }

        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.SetPosition(0, gravityBody.GetPosition(0) + (gravityBody.GetPosition(1) + gravityBody.GetPosition(2) * 2d + gravityBody.GetPosition(3) * 2d + gravityBody.GetPosition(4)) * (dt / 6d));
            gravityBody.SetVelocity(0, gravityBody.GetVelocity(0) + (gravityBody.GetVelocity(1) + gravityBody.GetVelocity(2) * 2d + gravityBody.GetVelocity(3) * 2d + gravityBody.GetVelocity(4)) * (dt / 6d));
        }
    }
}