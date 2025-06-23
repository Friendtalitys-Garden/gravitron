using System;
using System.Collections.Generic;
using System.Linq;

public class GravitySystem
{
    public LinkedList<GravityBody> gravityBodies;

    private Vector[] CalculateAccelerations(GravityState gravityState)
    {
        int size = gravityBodies.Count;

        Vector[,] acceleration_matrix = new Vector[size, size];

        // Top right half of matrix (from a to b), without masses multiplied
        for (int a = 0; a < size; a++)
        {
            for (int b = a + 1; b < size; b++)
            {
                Vector diff = gravityState.positions[b] - gravityState.positions[a]; // Vector from a to b

                acceleration_matrix[a, b] = diff * 0.000000000066743d * Math.Pow(-1.5, diff * diff);
            }
        }

        LinkedListNode<GravityBody> cur_a, cur_b;

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

        GravityState y = new GravityState(gravityBodies);

        GravityState k1 = y.derivative(CalculateAccelerations(y), GetAngularAccelerations());
    }
}