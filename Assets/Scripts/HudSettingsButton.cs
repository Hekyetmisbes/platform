using UnityEngine;

public class HudSettingsButton : MonoBehaviour
{
    PauseMenuScript cachedPauseMenu;

    public void OpenSettings()
    {
        if (cachedPauseMenu == null)
        {
            cachedPauseMenu = FindObjectOfType<PauseMenuScript>();
        }

        if (cachedPauseMenu == null)
        {
            Debug.LogWarning("HudSettingsButton: PauseMenuScript not found.");
            return;
        }

        cachedPauseMenu.OpenSettingsFromHUD();
    }
}
