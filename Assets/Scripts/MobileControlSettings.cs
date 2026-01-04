using System;
using UnityEngine;

public enum MobileControlMode
{
    Buttons = 0,
    Joystick = 1
}

public static class MobileControlSettings
{
    const string PrefKey = "MobileControlMode";

    public static MobileControlMode CurrentMode { get; private set; } = MobileControlMode.Buttons;
    public static event Action<MobileControlMode> ModeChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Load();
    }

    public static void Load()
    {
        if (PlayerPrefs.HasKey(PrefKey))
        {
            int stored = PlayerPrefs.GetInt(PrefKey, (int)MobileControlMode.Buttons);
            bool isValid = stored == (int)MobileControlMode.Buttons || stored == (int)MobileControlMode.Joystick;

            if (isValid)
            {
                CurrentMode = (MobileControlMode)stored;
            }
            else
            {
                // Reset to a safe default when an unexpected value is found so UI never stays disabled.
                CurrentMode = MobileControlMode.Buttons;
                PlayerPrefs.SetInt(PrefKey, (int)CurrentMode);
                PlayerPrefs.Save();
            }
        }
        else
        {
            CurrentMode = MobileControlMode.Buttons;
        }
    }

    public static void SetMode(MobileControlMode mode)
    {
        // Guard against invalid enum casts so we never write unusable values.
        if (mode != MobileControlMode.Buttons && mode != MobileControlMode.Joystick)
        {
            mode = MobileControlMode.Buttons;
        }

        if (CurrentMode == mode)
        {
            return;
        }
        CurrentMode = mode;
        PlayerPrefs.SetInt(PrefKey, (int)mode);
        PlayerPrefs.Save();
        ModeChanged?.Invoke(mode);
    }
}
