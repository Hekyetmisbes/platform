using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Text elemanını atayın. Eğer TMP kullanıyorsanız TextMeshProUGUI kullanın.
    private float countdownTime = 3f; // Geri sayım başlangıç değeri.
    [SerializeField] private GameObject gameplayElements; // Oyun elemanlarını buraya ekleyin.

    [SerializeField] private GameObject timeUI;

    private bool isCountdownActive;

    private void Start()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
        StartCoroutine(ShowTimesCoroutine());
    }

    private IEnumerator ShowTimesCoroutine()
    {
        yield return StartCoroutine(StartCountdown(
            countdownTime,
            timeUI,
            null,
            false
        ));

        isCountdownActive = true;

        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        yield return StartCoroutine(StartCountdown(
            countdownTime,
            countdownText.gameObject,
            countdownText,
            true
        ));

        countdownText.gameObject.SetActive(true);
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        isCountdownActive = false;

        countdownText.gameObject.SetActive(false);
        gameplayElements.SetActive(true);
    }

    private IEnumerator StartCountdown(float duration, GameObject uiElement, TextMeshProUGUI textElement, bool updateText)
    {
        int countdown = Mathf.CeilToInt(duration);
        uiElement.SetActive(true);

        while (countdown > 0)
        {
            if (updateText && textElement != null)
            {
                textElement.text = countdown.ToString(); // Yazıyı güncelle.
            }
            yield return new WaitForSeconds(1f); // 1 saniye bekle.
            countdown--;
        }
        uiElement.SetActive(false); // UI öğesini pasif yap.
    }

    public bool IsCountdownActive()
    {
        return isCountdownActive;
    }
}
