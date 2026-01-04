using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI button for left/right movement with visual and haptic feedback.
public class TouchHorizontalButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField, Range(-1f, 1f)] private float horizontalValue = 1f;

    [Header("Visual Feedback")]
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f);

    [Header("Haptic Feedback")]
    [SerializeField] private bool enableHaptic = true;

    private Vector3 normalScale;
    private Color normalColor;
    private Image imageComponent;

    private void Start()
    {
        normalScale = transform.localScale;
        imageComponent = GetComponent<Image>();
        if (imageComponent != null)
        {
            normalColor = imageComponent.color;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ApplyPressedVisuals();
        TriggerHapticFeedback();

        var im = InputManager.Instance;
        if (im == null) return;
        im.SetUIHorizontal(horizontalValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetVisuals();
        ClearHorizontal();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisuals();
        ClearHorizontal();
    }

    // Sprite/collider support (world-space buttons)
    void OnMouseDown()
    {
        ApplyPressedVisuals();
        TriggerHapticFeedback();

        var im = InputManager.Instance;
        if (im == null) return;
        im.SetUIHorizontal(horizontalValue);
    }

    void OnMouseUp()
    {
        ResetVisuals();
        ClearHorizontal();
    }

    void OnMouseExit()
    {
        ResetVisuals();
        ClearHorizontal();
    }

    void OnDisable()
    {
        ResetVisuals();
        ClearHorizontal();
    }

    void ClearHorizontal()
    {
        var im = InputManager.Instance;
        if (im == null) return;
        im.SetUIHorizontal(0f);
    }

    private void ApplyPressedVisuals()
    {
        transform.localScale = normalScale * pressScale;
        if (imageComponent != null)
        {
            imageComponent.color = pressedColor;
        }
    }

    private void ResetVisuals()
    {
        transform.localScale = normalScale;
        if (imageComponent != null)
        {
            imageComponent.color = normalColor;
        }
    }

    private void TriggerHapticFeedback()
    {
        if (!enableHaptic) return;

        #if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
        #endif
    }
}
