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
            CurrentMode = (MobileControlMode)PlayerPrefs.GetInt(PrefKey, (int)MobileControlMode.Buttons);
        }
        else
        {
            CurrentMode = MobileControlMode.Buttons;
        }
    }

    public static void SetMode(MobileControlMode mode)
    {
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
