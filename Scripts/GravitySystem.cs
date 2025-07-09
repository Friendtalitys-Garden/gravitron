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
				acceleration_matrix[a, b] = acceleration_matrix[a, b] * cur_b.Value.mass;

				cur_b = cur_b.Next;
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

	public static void CalculateOrbitParameters(GravityBody primary, GravityBody secondary)
	{
		if (secondary.mass < primary.mass * 0.01d)
		{
			// Not a binary system
			primary.isBinary = false;
			secondary.isBinary = false;
			secondary.frameOfReference = primary;

			// Relative position
			Vector r = secondary.position - primary.position;

			// inverse Distance since we only divide by distance
			double id = Math.Pow(r * r, -0.5d);

			// Relative velocity
			Vector v = secondary.velocity - primary.velocity;

			// Gravitational Parameter
			double gm = 6.6743e-11d * primary.mass;

			// Specific mechanical energy
			double sme = v * v * 0.5d - gm * id;

			secondary.a = -0.5d * gm / sme;

			secondary.sphereOfInfluence = secondary.a * Math.Pow(secondary.mass / primary.mass, 0.4d);

			secondary.h = r.x * v.y - r.y * v.x;

			// Eccentricity vector
			// Points from apoapsis to periapsis
			// Its length is the eccentricity
			double ex = v.y * secondary.h / gm - r.x * id;
			double ey = -v.x * secondary.h / gm - r.y * id;

			secondary.e = Math.Sqrt(ex * ex + ey * ey);

			// The Angle of the eccentricity vector is the argument of periapsis
			secondary.p = Math.Atan2(ey, ex);

			secondary.t = Math.Tau * Math.Sqrt(secondary.a * secondary.a * secondary.a / gm);
		}
		else
		{
			// Binary system
			BaryCenter baryCenter = new()
			{
				mass = primary.mass + secondary.mass,
				position = (primary.position * primary.mass + secondary.position * secondary.mass) / (primary.mass + secondary.mass),
				velocity = (primary.velocity * primary.mass + secondary.velocity * secondary.mass) / (primary.mass + secondary.mass),
				primary = primary,
				secondary = secondary
			};
			CalculateOrbitParameters(primary.frameOfReference, baryCenter);
			primary.frameOfReference = baryCenter;
			secondary.frameOfReference = baryCenter;
			primary.isBinary = true;
			secondary.isBinary = true;
			primary.binaryPartner = secondary;
			secondary.binaryPartner = primary;
			primary.isPrimary = true;

			// Relative position
			Vector r = secondary.position - baryCenter.position;

			// inverse Distance since we only divide by distance
			double id = Math.Pow(r * r, -0.5d);

			// Relative velocity
			Vector v = secondary.velocity - baryCenter.velocity;

			// Gravitational Parameter
			double gm = 6.6743e-11d * (primary.mass + secondary.mass);

			// Specific mechanical energy
			double sme = v * v * 0.5d - gm * id;

			double a = -0.5d * gm / sme;
			primary.a = a * secondary.mass / (primary.mass + secondary.mass);
			secondary.a = a * primary.mass / (primary.mass + secondary.mass);

			double h = r.x * v.y - r.y * v.x;
			primary.h = h;
			secondary.h = h;

			// Eccentricity vector
			// Points from apoapsis to periapsis
			// Its length is the eccentricity
			double ex = v.y * h / gm - r.x * id;
			double ey = -v.x * h / gm - r.y * id;

			double e = Math.Sqrt(ex * ex + ey * ey);
			primary.e = e;
			secondary.e = e;

			// The Angle of the eccentricity vector is the argument of periapsis
			double p = Math.Atan2(ey, ex);
			primary.p = p + Math.PI;
			secondary.p = p;
		}
	}

	public void CalculateSphereOfInfluences()
	{
		// First body (Sun) is considered the root body
		LinkedListNode<GravityBody> curBody = gravityBodies.First;
		curBody.Value.sphereOfInfluence = double.PositiveInfinity;
		curBody.Value.frameOfReference = curBody.Value;

		while (curBody != null)
		{
			// The current body is the secondary body,
			// the list is traversed backwards
			// to find the first larger body
			// whose sphere of influence contains the current
			LinkedListNode<GravityBody> curPrimary = curBody.Previous;
			while (curPrimary != null)
			{
				// Relative position
				Vector r = curBody.Value.position - curPrimary.Value.position;

				if (r * r < curPrimary.Value.sphereOfInfluence * curPrimary.Value.sphereOfInfluence)
				{
					CalculateOrbitParameters(curPrimary.Value, curBody.Value);
					break;
				}

				curPrimary = curPrimary.Previous;
			}

			curBody = curBody.Next;
		}
	}
}
