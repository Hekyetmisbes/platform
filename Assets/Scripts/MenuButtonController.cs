using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene(1);
    }

    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }
}
