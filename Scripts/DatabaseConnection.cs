using Godot;
using System;
using Npgsql;
using System.Linq;

public partial class DatabaseConnection : Node2D
{
	NpgsqlConnection connection;

	public override void _Ready()
	{
		string host = "localhost";
		string port = "5431";
		string database = "postgres";
		string username = "postgres";
		string password = "1234";

		// SETUP
		connection = Setup(host, port, database, username, password);
	}

	static NpgsqlConnection Setup(string host, string port, string database, string username, string password)
	{
		string connectionString = $"Host={host};Port={port};Database={database};User Id={username};Password={password};";

		try
		{
			using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
			connection.Open();

			return connection;
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Connection Fehler: {ex.Message}");
		}

		return null;
	}

	void Insert(Rocket rocket)
	{
		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand(@"
			INSERT INTO rockets (id, width, height, sprite, x, y, velocity, mass) 
			VALUES (@value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8)
			", connection);
			cmd.Parameters.AddWithValue("@value1", Guid.NewGuid());
			cmd.Parameters.AddWithValue("@value2", rocket.width);
			cmd.Parameters.AddWithValue("@value3", rocket.height);
			cmd.Parameters.AddWithValue("@value4", rocket.sprite);
			cmd.Parameters.AddWithValue("@value5", rocket.position.x);
			cmd.Parameters.AddWithValue("@value6", rocket.position.y);
			cmd.Parameters.AddWithValue("@value7", rocket.velocity);
			cmd.Parameters.AddWithValue("@value8", rocket.mass);

			int rowsAffected = cmd.ExecuteNonQuery();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}
		catch (Exception ex)
		{
			GD.Print($"Fehler System: {ex.Message}");
		}
	}

	void Insert(Planet planet)
	{
		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand(@"
			INSERT INTO planets (id, radius, sprite, x, y, velocity, mass) 
			VALUES (@value1, @value2, @value3, @value4, @value5, @value6, @value7)
			", connection);
			cmd.Parameters.AddWithValue("@value1", Guid.NewGuid());
			cmd.Parameters.AddWithValue("@value2", planet.radius);
			cmd.Parameters.AddWithValue("@value3", planet.sprite);
			cmd.Parameters.AddWithValue("@value4", planet.position.x);
			cmd.Parameters.AddWithValue("@value5", planet.position.y);
			cmd.Parameters.AddWithValue("@value6", planet.velocity);
			cmd.Parameters.AddWithValue("@value7", planet.mass);

			int rowsAffected = cmd.ExecuteNonQuery();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}
		catch (Exception ex)
		{
			GD.Print($"Fehler System: {ex.Message}");
		}
	}

	Rocket[] ReadRockets()
	{
		Rocket[] rockets = [];

		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM rockets", connection);
			using NpgsqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Rocket rocket = new Rocket();
				rocket.id = (Guid)reader["id"];

				rocket.width = (double)reader["width"];
				rocket.height = (double)reader["height"];

				rocket.sprite = (string)reader["sprite"];

				rocket.position.x = (double)reader["x"];
				rocket.position.y = (double)reader["y"];

				rocket.velocity = (Vector)reader["velocity"];
				rocket.mass = (double)reader["mass"];

				_ = rockets.Append(rocket);
			}
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}

		return rockets;
	}

	Planet[] ReadPlanets()
	{
		Planet[] planets = [];

		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM planets", connection);
			using NpgsqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Planet planet = new Planet();
				planet.id = (Guid)reader["id"];

				planet.radius = (double)reader["radius"];
				planet.sprite = (string)reader["sprite"];

				planet.position.x = (double)reader["x"];
				planet.position.y = (double)reader["y"];

				planet.velocity = (Vector)reader["velocity"];
				planet.mass = (double)reader["mass"];

				_ = planets.Append(planet);
			}
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}

		return planets;
	}

	void Update(Rocket rocket)
	{
		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand(@"
			UPDATE rockets
			SET x = @new_x,
    			y = @new_y,
    			velocity = @new_velocity,
				mass = @new_mass
			WHERE id = @condition;
			", connection);

			cmd.Parameters.AddWithValue("@condition", rocket.id);

			cmd.Parameters.AddWithValue("@new_x", rocket.position.x);
			cmd.Parameters.AddWithValue("@new_y", rocket.position.y);
			cmd.Parameters.AddWithValue("@new_velocity", rocket.velocity);
			cmd.Parameters.AddWithValue("@new_mass", rocket.mass);

			int rowsAffected = cmd.ExecuteNonQuery();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}
		catch (Exception ex)
		{
			GD.Print($"Fehler System: {ex.Message}");
		}
	}

	void Update(Planet planet)
	{
		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand(@"
			UPDATE planets
			SET x = @new_x,
    			y = @new_y,
    			velocity = @new_velocity
			WHERE id = @condition;
			", connection);

			cmd.Parameters.AddWithValue("@condition", planet.id);

			cmd.Parameters.AddWithValue("@new_x", planet.position.x);
			cmd.Parameters.AddWithValue("@new_y", planet.position.y);
			cmd.Parameters.AddWithValue("@new_velocity", planet.velocity);

			int rowsAffected = cmd.ExecuteNonQuery();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}
		catch (Exception ex)
		{
			GD.Print($"Fehler System: {ex.Message}");
		}
	}

	void Delete(Rocket rocket)
	{
		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM rockets WHERE id = @condition", connection);
			cmd.Parameters.AddWithValue("@condition", rocket.id);

			int rowsAffected = cmd.ExecuteNonQuery();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}
		catch (Exception ex)
		{
			GD.Print($"Fehler System: {ex.Message}");
		}
	}

	void Delete(Planet planet)
	{
		try
		{
			using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM planets WHERE id = @condition", connection);
			cmd.Parameters.AddWithValue("@condition", planet.id);

			int rowsAffected = cmd.ExecuteNonQuery();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Fehler DB: {ex.Message}");
		}
		catch (Exception ex)
		{
			GD.Print($"Fehler System: {ex.Message}");
		}
	}
}
