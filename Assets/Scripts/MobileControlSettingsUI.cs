using UnityEngine;

public class MobileControlSettingsUI : MonoBehaviour
{
    [SerializeField] GameObject joystickSelectedIndicator;
    [SerializeField] GameObject buttonsSelectedIndicator;

    void OnEnable()
    {
        Refresh();
    }

    public void SelectJoystick()
    {
        MobileControlSettings.SetMode(MobileControlMode.Joystick);
        Refresh();
    }

    public void SelectButtons()
    {
        MobileControlSettings.SetMode(MobileControlMode.Buttons);
        Refresh();
    }

    void Refresh()
    {
        var mode = MobileControlSettings.CurrentMode;
        if (joystickSelectedIndicator != null) joystickSelectedIndicator.SetActive(mode == MobileControlMode.Joystick);
        if (buttonsSelectedIndicator != null) buttonsSelectedIndicator.SetActive(mode == MobileControlMode.Buttons);
    }
}
