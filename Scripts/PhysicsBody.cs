public abstract class PhysicsBody
{
	public Vector position;
	public Vector position_k1;
	public Vector position_k2;
	public Vector position_k3;
	public Vector position_k4;
	public Vector position_save;

	public Vector velocity;
	public Vector velocity_k1;
	public Vector velocity_k2;
	public Vector velocity_k3;
	public Vector velocity_k4;
	public Vector velocity_save;

	public double angle;
	public double angularVelocity;

	public Vector GetPosition(int i)
	{
		return i switch
		{
			0 => position,
			1 => position_k1,
			2 => position_k2,
			3 => position_k3,
			4 => position_k4,
			_ => position,
		};
	}

	public Vector GetVelocity(int i)
	{
		return i switch
		{
			0 => velocity,
			1 => velocity_k1,
			2 => velocity_k2,
			3 => velocity_k3,
			4 => velocity_k4,
			_ => velocity,
		};
	}

	public void SetPosition(int i, Vector p)
	{
		switch (i)
		{
			case 0:
				position = p;
				break;
			case 1:
				position_k1 = p;
				break;
			case 2:
				position_k2 = p;
				break;
			case 3:
				position_k3 = p;
				break;
			case 4:
				position_k4 = p;
				break;

			default:
				position = p;
				break;
		}
	}

	public void SetVelocity(int i, Vector v)
	{
		switch (i)
		{
			case 0:
				velocity = v;
				break;
			case 1:
				velocity_k1 = v;
				break;
			case 2:
				velocity_k2 = v;
				break;
			case 3:
				velocity_k3 = v;
				break;
			case 4:
				velocity_k4 = v;
				break;

			default:
				velocity = v;
				break;
		}
	}
}
