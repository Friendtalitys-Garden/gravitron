using System;
using Godot;

public class Planet : GravityBody
{
	public Guid id;
	public string name;
	public double radius;
	public string sprite;

	public Texture2D texture;
}
