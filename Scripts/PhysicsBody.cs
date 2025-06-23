public abstract class PhysicsBody
{
	public Vector position;
	public Vector position_k1;
	public Vector position_k2;
	public Vector position_k3;
	public Vector position_k4;
	
	public Vector velocity;
	public Vector velocity_k1;
	public Vector velocity_k2;
	public Vector velocity_k3;
	public Vector velocity_k4;
	
	public double angle;
	public double angularVelocity;
	
	public Vector GetPosition(int i)
	{
		switch (i) {
			case 0: 
			return position;
			case 1:
			return position_k1;
			case 2:
			return position_k2;
			case 3:
			return position_k3;
			case 4:
			return position_k4;

			default: 
			return position;
		}
	}

	public Vector GetVelocity(int i)
	{
		switch (i) {
			case 0: 
			return velocity;
			case 1:
			return velocity_k1;
			case 2:
			return velocity_k2;
			case 3:
			return velocity_k3;
			case 4:
			return velocity_k4;

			default: 
			return velocity;
		}
	}

	public void SetPosition(int i, Vector p)
	{
		switch (i) {
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
		switch (i) {
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
