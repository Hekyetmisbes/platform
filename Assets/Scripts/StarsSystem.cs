using System;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// DatabaseHandler sınıfı, veritabanı işlemlerini kapsüller
public class DatabaseHandler
{
    private readonly string connectionString;

    public DatabaseHandler(string dbPath)
    {
        connectionString = $"Data Source={dbPath}";
    }

    public object ExecuteScalar(string query, params (string, object)[] parameters)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Item1, param.Item2);
                }
                return command.ExecuteScalar();
            }
        }
    }

    public void ExecuteNonQuery(string query, params (string, object)[] parameters)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Item1, param.Item2);
                }
                command.ExecuteNonQuery();
            }
        }
    }

    public SqliteDataReader ExecuteReader(string query, params (string, object)[] parameters)
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = new SqliteCommand(query, connection);
        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Item1, param.Item2);
        }
        return command.ExecuteReader(); // Connection will remain open for the reader
    }
}

// StarsCalculator sınıfı, yıldız hesaplama işlemlerini yönetir
public class StarsCalculator
{
    private readonly DatabaseHandler dbHandler;

    public StarsCalculator(DatabaseHandler dbHandler)
    {
        this.dbHandler = dbHandler;
    }

    public int CalculateStars(int sceneNumber, float finishTime)
    {
        string query = "SELECT StarTime1, StarTime2, StarTime3 FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
        using (var reader = dbHandler.ExecuteReader(query, ("@BolumNumarasi", sceneNumber)))
        {
            if (reader.Read())
            {
                float starTime1 = reader.GetFloat(0);
                float starTime2 = reader.GetFloat(1);
                float starTime3 = reader.GetFloat(2);

                if (finishTime <= starTime1) return 3;
                if (finishTime <= starTime2) return 2;
                if (finishTime <= starTime3) return 1;
            }
        }
        return 0;
    }

    public void UpdateStars(int sceneNumber, int newStars)
    {
        string query = "SELECT YildizSayisi FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
        int currentStars = Convert.ToInt32(dbHandler.ExecuteScalar(query, ("@BolumNumarasi", sceneNumber)) ?? 0);

        if (newStars > currentStars)
        {
            string updateQuery = "UPDATE BolumSureleri SET YildizSayisi = @YildizSayisi WHERE BolumNumarasi = @BolumNumarasi;";
            dbHandler.ExecuteNonQuery(updateQuery, ("@YildizSayisi", newStars), ("@BolumNumarasi", sceneNumber));
            Debug.Log($"Stars updated to {newStars} for scene {sceneNumber}.");
        }
        else
        {
            Debug.Log("New stars are not greater than current stars. No update performed.");
        }
    }
}

// StarsSystem sınıfı, oyun mantığını ve yıldız sistemini yönetir
public class StarsSystem : MonoBehaviour
{
    [SerializeField] private string databasePath = "StarsDatabase.db";
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Timer timer;

    private DatabaseHandler dbHandler;
    private StarsCalculator starsCalculator;
    private int sceneNumber;

    void Start()
    {
        dbHandler = new DatabaseHandler(databasePath);
        starsCalculator = new StarsCalculator(dbHandler);
        sceneNumber = SceneManager.GetActiveScene().buildIndex - 2;
        Debug.Log($"Scene number is {sceneNumber}.");
    }

    void Update()
    {
        if (playerController != null && playerController.IsFinish)
        {
            float finishTime = timer.GetTimeF();
            Debug.Log($"Player finish time: {finishTime}");

            UpdateFinishTime(sceneNumber, finishTime);
        }
    }

    private void UpdateFinishTime(int sceneNumber, float finishTime)
    {
        string selectQuery = "SELECT PlayerFinishTime FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
        object result = dbHandler.ExecuteScalar(selectQuery, ("@BolumNumarasi", sceneNumber));

        if (result == null || finishTime < Convert.ToSingle(result))
        {
            string updateQuery = "UPDATE BolumSureleri SET PlayerFinishTime = @PlayerFinishTime WHERE BolumNumarasi = @BolumNumarasi;";
            dbHandler.ExecuteNonQuery(updateQuery, ("@PlayerFinishTime", finishTime), ("@BolumNumarasi", sceneNumber));

            int stars = starsCalculator.CalculateStars(sceneNumber, finishTime);
            starsCalculator.UpdateStars(sceneNumber, stars);
        }
        else
        {
            Debug.Log("Finish time is not better. No update performed.");
        }
    }

    public int GetStars()
    {
        return starsCalculator.CalculateStars(sceneNumber, timer.GetTimeF());
    }

    // Yeni Metod: Tüm Bölümlerin Yıldız Bilgilerini Çekme
    public Dictionary<int, int> GetAllStars()
    {
        string query = "SELECT BolumNumarasi, YildizSayisi FROM BolumSureleri;";
        Dictionary<int, int> starsData = new Dictionary<int, int>();

        using (var connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int bolumNumarasi = reader.GetInt32(0);
                    int yildizSayisi = reader.GetInt32(1);
                    starsData[bolumNumarasi] = yildizSayisi;
                }
            }
        }

        return starsData;
    }
}
