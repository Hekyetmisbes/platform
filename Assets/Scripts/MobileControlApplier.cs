using UnityEngine;

public class MobileControlApplier : MonoBehaviour
{
    [SerializeField] GameObject buttonsRoot;
    [SerializeField] GameObject joystickRoot;
    [SerializeField] VirtualJoystick joystick;

    void OnEnable()
    {
        MobileControlSettings.ModeChanged += HandleModeChanged;
        Apply();
    }

    void OnDisable()
    {
        MobileControlSettings.ModeChanged -= HandleModeChanged;
    }

    void Start()
    {
        Apply();
    }

    void HandleModeChanged(MobileControlMode mode)
    {
        Apply();
    }

    public void Apply()
    {
        var mode = MobileControlSettings.CurrentMode;

        if (buttonsRoot != null) buttonsRoot.SetActive(mode == MobileControlMode.Buttons);
        if (joystickRoot != null) joystickRoot.SetActive(mode == MobileControlMode.Joystick);

        var inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.SetJoystick(mode == MobileControlMode.Joystick ? joystick : null);
        }
    }
}
