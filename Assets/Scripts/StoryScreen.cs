using UnityEngine;
using TMPro;
using System.Collections;

public class StoryScreen : MonoBehaviour
{
    public GameObject storyScreen; // Hikaye ekranı paneli
    public TextMeshProUGUI storyText; // Hikaye metni
    public string[] storyLines; // Hikaye satırları
    public float textSpeed = 0.05f;

    private int currentLine = 0;
    private Coroutine currentTypingCoroutine;

    void Start()
    {
        storyScreen.SetActive(false); // Hikaye ekranı başlangıçta kapalı olsun
    }

    public void ShowStoryScreen()
    {
        // Eğer başka bir yazdırma işlemi varsa, durdur
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
        }

        storyScreen.SetActive(true); // Hikaye ekranını aç
        currentLine = 0; // Başlangıç satırına dön
        storyText.text = ""; // Metni temizle
        currentTypingCoroutine = StartCoroutine(TypeText(storyLines[currentLine])); // İlk satırı yazdır
    }

    IEnumerator TypeText(string line)
    {
        storyText.text = ""; // Mevcut metni temizle
        foreach (char letter in line.ToCharArray())
        {
            storyText.text += letter; // Harf harf yazdır
            yield return new WaitForSeconds(textSpeed);
        }

        currentTypingCoroutine = null; // Yazdırma işlemi bittiğinde Coroutine'i sıfırla
    }

    public void CloseStoryScreen()
    {
        // Eğer bir yazdırma işlemi devam ediyorsa, durdur
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
            currentTypingCoroutine = null;
        }

        storyText.text = ""; // Metni temizle
        storyScreen.SetActive(false); // Hikaye ekranını kapat
    }
}
