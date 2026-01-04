using System;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        const string query = "SELECT StarTime1, StarTime2, StarTime3 FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
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
        const string query = "SELECT YildizSayisi FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
        int currentStars = Convert.ToInt32(dbHandler.ExecuteScalar(query, ("@BolumNumarasi", sceneNumber)) ?? 0);

        if (newStars > currentStars)
        {
            const string updateQuery = "UPDATE BolumSureleri SET YildizSayisi = @YildizSayisi WHERE BolumNumarasi = @BolumNumarasi;";
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
    private bool finishProcessed;
    private bool starTimesLoaded;
    private float starTime1;
    private float starTime2;
    private float starTime3;

    void Start()
    {
        dbHandler = new DatabaseHandler(databasePath);
        starsCalculator = new StarsCalculator(dbHandler);
        sceneNumber = SceneManager.GetActiveScene().buildIndex - 2;
        LoadStarTimes();
        Debug.Log($"Scene number is {sceneNumber}.");
    }

    void Update()
    {
        if (!finishProcessed && playerController != null && playerController.IsFinish)
        {
            finishProcessed = true;
            float finishTime = timer.GetTimeF();
            Debug.Log($"Player finish time: {finishTime}");

            UpdateFinishTime(sceneNumber, finishTime);
        }
    }

    private void LoadStarTimes()
    {
        const string query = "SELECT StarTime1, StarTime2, StarTime3 FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
        using (var reader = dbHandler.ExecuteReader(query, ("@BolumNumarasi", sceneNumber)))
        {
            if (reader.Read())
            {
                starTime1 = reader.GetFloat(0);
                starTime2 = reader.GetFloat(1);
                starTime3 = reader.GetFloat(2);
                starTimesLoaded = true;
            }
        }
    }

    private int CalculateStarsLocal(float finishTime)
    {
        if (!starTimesLoaded)
        {
            return starsCalculator.CalculateStars(sceneNumber, finishTime);
        }

        if (finishTime <= starTime1) return 3;
        if (finishTime <= starTime2) return 2;
        if (finishTime <= starTime3) return 1;
        return 0;
    }

    private void UpdateFinishTime(int sceneNumber, float finishTime)
    {
        const string selectQuery = "SELECT PlayerFinishTime FROM BolumSureleri WHERE BolumNumarasi = @BolumNumarasi;";
        object result = dbHandler.ExecuteScalar(selectQuery, ("@BolumNumarasi", sceneNumber));

        if (result == null || finishTime < Convert.ToSingle(result))
        {
            const string updateQuery = "UPDATE BolumSureleri SET PlayerFinishTime = @PlayerFinishTime WHERE BolumNumarasi = @BolumNumarasi;";
            dbHandler.ExecuteNonQuery(updateQuery, ("@PlayerFinishTime", finishTime), ("@BolumNumarasi", sceneNumber));

            int stars = CalculateStarsLocal(finishTime);
            starsCalculator.UpdateStars(sceneNumber, stars);
        }
        else
        {
            Debug.Log("Finish time is not better. No update performed.");
        }
    }

    public int GetStars()
    {
        return CalculateStarsLocal(timer.GetTimeF());
    }

    // Yeni Metod: Tüm Bölümlerin Yıldız Bilgilerini Çekme
    public Dictionary<int, int> GetAllStars()
    {
        const string query = "SELECT BolumNumarasi, YildizSayisi FROM BolumSureleri;";
        Dictionary<int, int> starsData = new Dictionary<int, int>();

        DatabaseHandler handler = GetOrCreateHandler();

        using (var reader = handler.ExecuteReader(query))
        {
            while (reader.Read())
            {
                int bolumNumarasi = reader.GetInt32(0);
                int yildizSayisi = reader.GetInt32(1);
                starsData[bolumNumarasi] = yildizSayisi;
            }
        }

        return starsData;
    }

    private DatabaseHandler GetOrCreateHandler()
    {
        if (dbHandler == null)
        {
            dbHandler = new DatabaseHandler(databasePath);
        }
        return dbHandler;
    }

    private void OnDestroy()
    {
        if (dbHandler != null)
        {
            dbHandler.Dispose();
            dbHandler = null;
        }
    }
}
