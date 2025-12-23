using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private StarsSystem starsSystem;

    [SerializeField] private Transform doorsParent; // Doors alt nesneleri için parent GameObject
    [SerializeField] private Transform starsParent; // Stars alt nesneleri için parent GameObject

    private GameObject[] doors; // Kapı nesneleri
    private List<GameObject[]> starGroups = new List<GameObject[]>(); // Yıldız grupları

    void Start()
    {
        InitializeDoorsAndStars();
        UpdateDoorStars();
        UpdateDoorLocks();
    }

    // Kapılar ve yıldız gruplarını hiyerarşiye göre eşleştir
    void InitializeDoorsAndStars()
    {
        // Doors ve Stars altındaki nesneleri al
        doors = new GameObject[doorsParent.childCount];

        for (int i = 0; i < doorsParent.childCount; i++)
        {
            doors[i] = doorsParent.GetChild(i).gameObject;

            // Stars altında her kapıya ait yıldız gruplarını listeye ekle
            Transform starGroup = starsParent.GetChild(i);
            GameObject[] starObjects = new GameObject[starGroup.childCount];

            for (int j = 0; j < starGroup.childCount; j++)
            {
                starObjects[j] = starGroup.GetChild(j).gameObject;
            }

            starGroups.Add(starObjects);
        }

        Debug.Log($"Kapılar ve yıldızlar başarıyla eşleştirildi: {doors.Length} kapı, {starGroups.Count} yıldız grubu.");
    }

    // Update the stars on doors
    void UpdateDoorStars()
    {
        // Get Stars Data from StarsSystem
        Dictionary<int, int> starsData = starsSystem.GetAllStars();

        for (int i = 0; i < doors.Length; i++)
        {
            int bolumNumarasi = i + 1;

            // Get Star Count for Door
            int yildizSayisi = starsData.ContainsKey(bolumNumarasi) ? starsData[bolumNumarasi] : 0;

            // Update Star Visuals for Door
            UpdateStarVisual(i, yildizSayisi);
        }
    }

    void UpdateDoorLocks()
    {
        Dictionary<int, int> starsData = starsSystem.GetAllStars();

        for (int i = 0; i < doors.Length; i++)
        {
            int levelNumber = i + 1;
            bool locked = false;

            for (int prevLevel = 1; prevLevel < levelNumber; prevLevel++)
            {
                int stars = starsData.ContainsKey(prevLevel) ? starsData[prevLevel] : 0;
                if (stars < 1)
                {
                    locked = true;
                    break;
                }
            }

            DoorController doorController = doors[i].GetComponent<DoorController>();
            if (doorController != null)
            {
                doorController.SetLocked(locked);
            }
        }
    }

    // Belirtilen kapının yıldız görsellerini güncelle
    void UpdateStarVisual(int doorIndex, int starCount)
    {
        // Hata kontrolü: Eğer yıldız grubu eksikse
        if (starGroups[doorIndex].Length == 0)
        {
            Debug.LogError($"Kapı {doorIndex + 1} için yıldız grubu eksik.");
            return;
        }

        // Tüm yıldız gruplarını pasif yap
        foreach (var starGroup in starGroups[doorIndex])
        {
            starGroup.SetActive(false);
        }

        // Yıldız sayısına uygun olan grubu aktif yap
        if (starCount >= 0 && starCount < starGroups[doorIndex].Length)
        {
            starGroups[doorIndex][starCount].SetActive(true);
            Debug.Log($"Kapı {doorIndex + 1} için {starCount} yıldız görseli gösteriliyor.");
        }
        else
        {
            Debug.LogWarning($"Kapı {doorIndex + 1} için geçersiz yıldız sayısı ({starCount}).");
        }
    }
}
