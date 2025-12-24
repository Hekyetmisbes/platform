using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Text elemanını atayın. Eğer TMP kullanıyorsanız TextMeshProUGUI kullanın.
    private float countdownTime = 3f; // Geri sayım başlangıç değeri.
    [SerializeField] private GameObject gameplayElements; // Oyun elemanlarını buraya ekleyin.
    [SerializeField] private GameObject[] extraGameplayElements; // Geri sayımdan sonra aktif olacak ek UI/objeler.

    [SerializeField] private GameObject timeUI;
    [SerializeField] private GameObject buttonsRoot;
    [SerializeField] private GameObject joystickRoot;
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private GameObject hudSettingsButton;
    [SerializeField] private bool autoFindControls = true;

    private bool isCountdownActive;
    private bool controlsUnlocked;

    private void Awake()
    {
        CacheControlReferences();
        ApplyControlMode(false);
    }

    private void OnEnable()
    {
        MobileControlSettings.ModeChanged += HandleModeChanged;
    }

    private void OnDisable()
    {
        MobileControlSettings.ModeChanged -= HandleModeChanged;
    }

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
        if (gameplayElements != null) gameplayElements.SetActive(true);
        if (extraGameplayElements != null)
        {
            for (int i = 0; i < extraGameplayElements.Length; i++)
            {
                var element = extraGameplayElements[i];
                if (element != null) element.SetActive(true);
            }
        }
        controlsUnlocked = true;
        ApplyControlMode(true);
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

    private void HandleModeChanged(MobileControlMode mode)
    {
        if (!controlsUnlocked)
        {
            return;
        }
        ApplyControlMode(true);
    }

    private void CacheControlReferences()
    {
        if (!autoFindControls)
        {
            return;
        }

        if (buttonsRoot == null)
        {
            buttonsRoot = FindInSceneByName("PlayButtons");
        }

        if (joystickRoot == null)
        {
            joystickRoot = FindInSceneByName("VirtualJoystickUI");
            if (joystickRoot == null)
            {
                joystickRoot = FindInSceneByName("VirtualJoystick");
            }
        }

        if (hudSettingsButton == null)
        {
            hudSettingsButton = FindInSceneByName("HudSettingsButton");
        }

        if (joystick == null && joystickRoot != null)
        {
            joystick = joystickRoot.GetComponentInChildren<VirtualJoystick>(true);
        }

        if (joystick == null)
        {
            joystick = FindInSceneVirtualJoystick();
        }
    }

    private void ApplyControlMode(bool allowActivate)
    {
        if (!allowActivate)
        {
            if (buttonsRoot != null) buttonsRoot.SetActive(false);
            if (joystickRoot != null) joystickRoot.SetActive(false);
            if (hudSettingsButton != null) hudSettingsButton.SetActive(false);
            var inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.SetJoystick(null);
            }
            return;
        }

        var mode = MobileControlSettings.CurrentMode;
        bool showButtons = mode == MobileControlMode.Buttons;
        bool showJoystick = mode == MobileControlMode.Joystick;

        if (buttonsRoot != null) buttonsRoot.SetActive(showButtons);
        if (joystickRoot != null) joystickRoot.SetActive(showJoystick);
        if (hudSettingsButton != null) hudSettingsButton.SetActive(true);

        var manager = InputManager.Instance;
        if (manager != null)
        {
            manager.SetJoystick(showJoystick ? joystick : null);
        }
    }

    private static GameObject FindInSceneByName(string name)
    {
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            var obj = allObjects[i];
            if (!obj.scene.IsValid())
            {
                continue;
            }
            if (obj.hideFlags != HideFlags.None)
            {
                continue;
            }
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    private static VirtualJoystick FindInSceneVirtualJoystick()
    {
        var allJoysticks = Resources.FindObjectsOfTypeAll<VirtualJoystick>();
        for (int i = 0; i < allJoysticks.Length; i++)
        {
            var found = allJoysticks[i];
            if (found == null)
            {
                continue;
            }
            if (!found.gameObject.scene.IsValid())
            {
                continue;
            }
            if (found.hideFlags != HideFlags.None)
            {
                continue;
            }
            return found;
        }
        return null;
    }
}
