public abstract class GravityBody : PhysicsBody
{
	public double mass;
	public Vector externalAcceleration;
	public double externalAngularAcceleration;

	// Orbit parameters

	// Semi-major axis
	public double a;

	// Specific angular momentum
	public double h;

	// Eccentricity
	public double e;

	// Argument of periapsis
	public double p;

	// Orbital period
	public double t;

	public GravityBody frameOfReference;
	public double sphereOfInfluence;
	public bool isBinary;
	public bool isPrimary;
	public GravityBody binaryPartner;
}
