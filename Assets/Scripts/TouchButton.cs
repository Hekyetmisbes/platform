using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Simple UI button bridge for touch devices with visual and haptic feedback.
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonAction
    {
        Jump,
        Restart
    }

    [SerializeField] private ButtonAction action = ButtonAction.Jump;

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

        if (action == ButtonAction.Jump)
        {
            im.PressUIJump();
        }
        else
        {
            im.PressUIRestart();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetVisuals();
    }

    // Sprite/collider support (world-space buttons)
    void OnMouseDown()
    {
        ApplyPressedVisuals();
        TriggerHapticFeedback();

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

    void OnMouseUp()
    {
        ResetVisuals();
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
