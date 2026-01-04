using UnityEngine;
using UnityEngine.EventSystems;

// Simple UI button bridge for touch devices.
public class TouchButton : MonoBehaviour, IPointerDownHandler
{
    public enum ButtonAction
    {
        Jump,
        Restart
    }

    [SerializeField] ButtonAction action = ButtonAction.Jump;

    public void OnPointerDown(PointerEventData eventData)
    {
        var im = InputManager.Instance;
        if (im == null) return;

        if (action == ButtonAction.Jump)
        {
            im.PressUIJump();
        }
        else
        {
            im.PressUIRestart();
        }
    }

    // Sprite/collider support (world-space buttons)
    void OnMouseDown()
    {
        var im = InputManager.Instance;
        if (im == null) return;

        if (action == ButtonAction.Jump)
        {
            im.PressUIJump();
        }
        else
        {
            im.PressUIRestart();
        }
    }
}
