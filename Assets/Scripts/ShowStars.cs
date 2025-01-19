using System.Collections;
using UnityEngine;

public class ShowStars : MonoBehaviour
{
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [SerializeField] private GameObject star0;

    [SerializeField] private StarsSystem starsSystem;

    [SerializeField] private PlayerController playerController;
    private int yildizSayisi = 0;

    [SerializeField] private GameObject player;

    // Update is called once per frame
    void Update()
    {
        yildizSayisi = starsSystem.GetStars();
        if(player.activeSelf)
        {
            ShowStarsMethod(yildizSayisi);
        }
    }

    private void ShowStarsMethod(int stars)
    {
        // Tüm yıldızları pasif yap
        star0.SetActive(false);
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        // Yıldız sayısına göre ilgili görseli aktif yap
        switch (stars)
        {
            case 3:
                star3.SetActive(true);
                break;
            case 2:
                star2.SetActive(true);
                break;
            case 1:
                star1.SetActive(true);
                break;
            default:
                star0.SetActive(true);
                break;
        }

        Debug.Log($"Yıldızlar güncellendi: {stars} yıldız");
    }
}
