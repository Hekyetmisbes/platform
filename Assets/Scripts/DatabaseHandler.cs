using System;
using System.Data;
using Mono.Data.Sqlite;

// DatabaseHandler sınıfı, veritabanı işlemlerini kapsüller
public class DatabaseHandler
{
    private readonly string connectionString;

    public DatabaseHandler(string dbPath)
    {
        connectionString = $"Data Source={dbPath}";
    }

    private static void ValidateParameters((string, object)[] parameters)
    {
        if (parameters == null) return;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parameters[i].Item1) || !parameters[i].Item1.StartsWith("@"))
            {
                throw new ArgumentException("Parameter names must be non-empty and start with '@'.");
            }
        }
    }

    private static SqliteCommand CreateCommand(string query, SqliteConnection connection, (string, object)[] parameters)
    {
        ValidateParameters(parameters);
        var command = new SqliteCommand(query, connection);
        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Item1, param.Item2 ?? DBNull.Value);
        }
        command.Prepare();
        return command;
    }

    public object ExecuteScalar(string query, params (string, object)[] parameters)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = CreateCommand(query, connection, parameters))
            {
                return command.ExecuteScalar();
            }
        }
    }

    public void ExecuteNonQuery(string query, params (string, object)[] parameters)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = CreateCommand(query, connection, parameters))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public SqliteDataReader ExecuteReader(string query, params (string, object)[] parameters)
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = CreateCommand(query, connection, parameters);
        // Close the connection when the reader is disposed.
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }
}
