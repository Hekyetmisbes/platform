using UnityEngine;
using UnityEngine.EventSystems;

// Simple UI joystick that outputs horizontal input in range [-1, 1].
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] RectTransform background;
    [SerializeField] RectTransform handle;
    [SerializeField, Range(0.1f, 300f)] float radius = 100f;

    public float Horizontal { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint))
        {
            Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);
            handle.anchoredPosition = clamped;
            Horizontal = Mathf.Clamp(clamped.x / radius, -1f, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
        Horizontal = 0f;
    }
}
