using UnityEngine;
using UnityEngine.EventSystems;

// UI button for left/right movement. Assign -1 for left, +1 for right.
public class TouchHorizontalButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField, Range(-1f, 1f)] float horizontalValue = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        var im = InputManager.Instance;
        if (im == null) return;
        im.SetUIHorizontal(horizontalValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ClearHorizontal();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClearHorizontal();
    }

    // Sprite/collider support (world-space buttons)
    void OnMouseDown()
    {
        var im = InputManager.Instance;
        if (im == null) return;
        im.SetUIHorizontal(horizontalValue);
    }

    void OnMouseUp()
    {
        ClearHorizontal();
    }

    void OnMouseExit()
    {
        ClearHorizontal();
    }

    void OnDisable()
    {
        ClearHorizontal();
    }

    void ClearHorizontal()
    {
        var im = InputManager.Instance;
        if (im == null) return;
        im.SetUIHorizontal(0f);
    }
}
