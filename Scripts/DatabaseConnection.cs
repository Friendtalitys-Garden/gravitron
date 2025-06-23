using Godot;
using System;
using Npgsql;

public partial class DatabaseConnection : Node2D
{
	public override void _Ready()
	{
		// SETUP
		string connectionString = Setup();

		// INSERT 
		//Insert(connectionString);

		// READ
		//Read(connectionString);

		// DELETE
		//Delete(connectionString);

		// UPDATE 
		Update(connectionString);
	}

	static string Setup()
	{
		string host = "localhost";
		string port = "5431";
		string database = "postgres";
		string username = "postgres";
		string password = "1234";

		string connectionString = $"Host={host};Port={port};Database={database};User Id={username};Password={password};";

		try
		{
			using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
			connection.Open();
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Database connection error: {ex.Message}");
		}

		return connectionString;
	}

	static void Insert(string connectionString)
	{
		try
		{
			using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
			connection.Open();

			using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO benutzer (name, email) VALUES (@value1, @value2)", connection);
			cmd.Parameters.AddWithValue("@value1", "Sax Sustermann");
			cmd.Parameters.AddWithValue("@value2", "sax@sustermann.de");

			int rowsAffected = cmd.ExecuteNonQuery();
			Console.WriteLine($"{rowsAffected} row(s) inserted");
		}
		catch (Npgsql.NpgsqlException ex)
		{
			Console.WriteLine($"Error inserting data: {ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"General error during insert operation: {ex.Message}");
		}
	}

	static void Read(string connectionString)
	{
		try
		{
			using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
			connection.Open();

			using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM benutzer WHERE name = 'Sax Sustermann'", connection);
			using NpgsqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				GD.Print(reader["email"], reader["name"]);
			}
		}
		catch (Npgsql.NpgsqlException ex)
		{
			GD.Print($"Database error: {ex.Message}");
		}
	}

	static void Delete(string connectionString)
	{
		try
		{
			using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
			connection.Open();

			using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM benutzer WHERE name ='Max Mustermann'", connection);

			int rowsAffected = cmd.ExecuteNonQuery();
			Console.WriteLine($"{rowsAffected} row(s) inserted");
		}
		catch (Npgsql.NpgsqlException ex)
		{
			Console.WriteLine($"Error inserting data: {ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"General error during insert operation: {ex.Message}");
		}
	}

	static void Update(string connectionString)
	{
		try
		{
			using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
			connection.Open();

			using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE benutzer SET name = @new_value WHERE email = @condition", connection);
			cmd.Parameters.AddWithValue("@new_value", "Sus Sustermann");
			cmd.Parameters.AddWithValue("@condition", "sax@sustermann.de");

			int rowsAffected = cmd.ExecuteNonQuery();
			Console.WriteLine($"{rowsAffected} row(s) updated");
		}
		catch (Npgsql.NpgsqlException ex)
		{
			Console.WriteLine($"Error updating data: {ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"General error during update operation: {ex.Message}");
		}
	}
}
