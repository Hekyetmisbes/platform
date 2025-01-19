using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;
    public TextMeshProUGUI loadingText;

    [SerializeField] private string sceneName = "Levels";

    void Start()
    {
        Time.timeScale = 1f;
    }

    public void Load()
    {
        LoadLevel(sceneName);
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AudioListener.volume = 0f;

        loadingScreen.SetActive(true);

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Yükleme tamamlanmadan sahneye geçişi engelle
        
        float fakeProgress = 0f;

        while (fakeProgress < 1f || !operation.isDone)
        {
            if (fakeProgress < operation.progress)
            {
                fakeProgress += Time.deltaTime * 0.8f; // İlerleme hızını ayarla
            }
            else if (operation.progress >= 0.9f) // Gerçek ilerleme tamamlandığında
            {
                fakeProgress = Mathf.MoveTowards(fakeProgress, 1f, Time.deltaTime * 0.1f);
            }

            progressBar.value = fakeProgress;
            loadingText.text = "Loading... " + (fakeProgress * 100).ToString("F0") + "%";

            // Yükleme tamamsa sahneye geçiş yap
            if (fakeProgress >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
