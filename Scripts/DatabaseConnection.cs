using Godot;
using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public partial class DatabaseConnection
{
	SqliteConnection connection;

	public void Setup()
	{
		string dbPath = "mydatabase.db";
		string fullPath = ProjectSettings.GlobalizePath(dbPath);

		bool createTables = !System.IO.File.Exists(fullPath);
		connection = new SqliteConnection($"Data Source={fullPath}");
		connection.Open();

		if (createTables)
			CreateTables();
	}

	public void CreateTables()
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = @"
			CREATE TABLE IF NOT EXISTS rockets (
				id TEXT PRIMARY KEY,
				name TEXT,
				width REAL,
				height REAL,
				sprite TEXT,
				x REAL,
				y REAL,
				velocity_x REAL,
				velocity_y REAL,
				mass REAL
			);
			CREATE TABLE IF NOT EXISTS planets (
				id TEXT PRIMARY KEY,
				name TEXT,
				radius REAL,
				sprite TEXT,
				x REAL,
				y REAL,
				velocity_x REAL,
				velocity_y REAL,
				mass REAL
			);";
		cmd.ExecuteNonQuery();
	}

	public void Insert(Rocket rocket)
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = @"
			INSERT INTO rockets (id, name, width, height, sprite, x, y, velocity_x, velocity_y, mass)
			VALUES ($id, $name, $width, $height, $sprite, $x, $y, $vx, $vy, $mass);
		";

		cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
		cmd.Parameters.AddWithValue("$name", rocket.name);
		cmd.Parameters.AddWithValue("$width", rocket.width);
		cmd.Parameters.AddWithValue("$height", rocket.height);
		cmd.Parameters.AddWithValue("$sprite", rocket.sprite);
		cmd.Parameters.AddWithValue("$x", rocket.position.x);
		cmd.Parameters.AddWithValue("$y", rocket.position.y);
		cmd.Parameters.AddWithValue("$vx", rocket.velocity.x);
		cmd.Parameters.AddWithValue("$vy", rocket.velocity.y);
		cmd.Parameters.AddWithValue("$mass", rocket.mass);

		cmd.ExecuteNonQuery();
	}

	public void Insert(Planet planet)
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = @"
			INSERT INTO planets (id, name, radius, sprite, x, y, velocity_x, velocity_y, mass)
			VALUES ($id, $name, $radius, $sprite, $x, $y, $vx, $vy, $mass);
		";

		cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
		cmd.Parameters.AddWithValue("$name", planet.name);
		cmd.Parameters.AddWithValue("$radius", planet.radius);
		cmd.Parameters.AddWithValue("$sprite", planet.sprite);
		cmd.Parameters.AddWithValue("$x", planet.position.x);
		cmd.Parameters.AddWithValue("$y", planet.position.y);
		cmd.Parameters.AddWithValue("$vx", planet.velocity.x);
		cmd.Parameters.AddWithValue("$vy", planet.velocity.y);
		cmd.Parameters.AddWithValue("$mass", planet.mass);

		cmd.ExecuteNonQuery();
	}

	public LinkedList<Rocket> ReadRockets()
	{
		LinkedList<Rocket> rockets = new();

		using var cmd = connection.CreateCommand();
		cmd.CommandText = "SELECT * FROM rockets ORDER BY mass DESC";
		using var reader = cmd.ExecuteReader();

		while (reader.Read())
		{
			var rocket = new Rocket
			{
				id = Guid.Parse(reader["id"].ToString()),
				name = reader["name"].ToString(),
				width = Convert.ToDouble(reader["width"]),
				height = Convert.ToDouble(reader["height"]),
				sprite = reader["sprite"].ToString(),
				position = new Vector(Convert.ToDouble(reader["x"]), Convert.ToDouble(reader["y"])),
				velocity = new Vector(Convert.ToDouble(reader["velocity_x"]), Convert.ToDouble(reader["velocity_y"])),
				mass = Convert.ToDouble(reader["mass"])
			};

			rockets.AddLast(rocket);
		}

		return rockets;
	}

	public LinkedList<Planet> ReadPlanets()
	{
		LinkedList<Planet> planets = new();

		using var cmd = connection.CreateCommand();
		cmd.CommandText = "SELECT * FROM planets ORDER BY mass DESC";
		using var reader = cmd.ExecuteReader();

		while (reader.Read())
		{
			var planet = new Planet
			{
				id = Guid.Parse(reader["id"].ToString()),
				name = reader["name"].ToString(),
				radius = Convert.ToDouble(reader["radius"]),
				sprite = reader["sprite"].ToString(),
				position = new Vector(Convert.ToDouble(reader["x"]), Convert.ToDouble(reader["y"])),
				velocity = new Vector(Convert.ToDouble(reader["velocity_x"]), Convert.ToDouble(reader["velocity_y"])),
				mass = Convert.ToDouble(reader["mass"])
			};

			planets.AddLast(planet);
		}

		return planets;
	}

	public void Update(Rocket rocket)
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = @"
			UPDATE rockets
			SET name = $name, x = $x, y = $y, velocity_x = $vx, velocity_y = $vy, mass = $mass
			WHERE id = $id;
		";

		cmd.Parameters.AddWithValue("$id", rocket.id.ToString());
		cmd.Parameters.AddWithValue("$name", rocket.name);
		cmd.Parameters.AddWithValue("$x", rocket.position.x);
		cmd.Parameters.AddWithValue("$y", rocket.position.y);
		cmd.Parameters.AddWithValue("$vx", rocket.velocity.x);
		cmd.Parameters.AddWithValue("$vy", rocket.velocity.y);
		cmd.Parameters.AddWithValue("$mass", rocket.mass);

		cmd.ExecuteNonQuery();
	}

	public void Update(Planet planet)
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = @"
			UPDATE planets
			SET name = $name, x = $x, y = $y, velocity_x = $vx, velocity_y = $vy
			WHERE id = $id;
		";

		cmd.Parameters.AddWithValue("$id", planet.id.ToString());
		cmd.Parameters.AddWithValue("$name", planet.name);
		cmd.Parameters.AddWithValue("$x", planet.position.x);
		cmd.Parameters.AddWithValue("$y", planet.position.y);
		cmd.Parameters.AddWithValue("$vx", planet.velocity.x);
		cmd.Parameters.AddWithValue("$vy", planet.velocity.y);

		cmd.ExecuteNonQuery();
	}

	public void Delete(Rocket rocket)
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = "DELETE FROM rockets WHERE id = $id";
		cmd.Parameters.AddWithValue("$id", rocket.id.ToString());
		cmd.ExecuteNonQuery();
	}

	public void Delete(Planet planet)
	{
		using var cmd = connection.CreateCommand();
		cmd.CommandText = "DELETE FROM planets WHERE id = $id";
		cmd.Parameters.AddWithValue("$id", planet.id.ToString());
		cmd.ExecuteNonQuery();
	}
}
