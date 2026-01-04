using UnityEngine;

/// <summary>
/// Legacy component that delegates to MobileControlManager.
/// Kept for backwards compatibility with existing scenes.
/// Consider using MobileControlManager directly instead.
/// </summary>
public class MobileControlApplier : MonoBehaviour
{
    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        var controlManager = MobileControlManager.Instance;
        if (controlManager != null)
        {
            controlManager.ApplyControlMode();
        }
    }
}
