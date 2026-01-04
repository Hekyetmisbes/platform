using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfig config;

    [Header("Camera Target")]
    [SerializeField] private Transform target;

    [Header("Camera Bounds")]
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 0f;
    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 78.5f;

    [Header("Look-Ahead Settings")]
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSpeed = 2f;
    [SerializeField] private Vector2 cameraOffset = new Vector2(0, 1);

    private float smoothing = 5f;
    private float smoothTime = 0.3f;
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    private float currentLookAhead = 0f;
    private Rigidbody2D targetRigidbody;

    void Start()
    {
        offset = transform.position - target.position;

        // Get the Rigidbody2D for velocity-based look-ahead
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody2D>();
        }

        // Load values from config if available
        if (config != null)
        {
            smoothing = config.cameraFollowSpeed;
            smoothTime = config.cameraSmoothTime;
            lookAheadDistance = config.lookAheadDistance;
            lookAheadSpeed = config.lookAheadSpeed;
            cameraOffset = new Vector2(config.cameraOffset.x, config.cameraOffset.y);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate look-ahead based on player velocity
        float targetLookAhead = 0f;
        if (targetRigidbody != null && Mathf.Abs(targetRigidbody.linearVelocity.x) > 0.1f)
        {
            targetLookAhead = Mathf.Sign(targetRigidbody.linearVelocity.x) * lookAheadDistance;
        }

        // Smooth look-ahead transition
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * lookAheadSpeed);

        // Calculate target position with look-ahead and offset
        Vector3 targetCamPos = new Vector3(
            target.position.x + currentLookAhead + cameraOffset.x,
            target.position.y + cameraOffset.y,
            transform.position.z
        );

        ChangeCamTransform(targetCamPos);
    }

    void ChangeCamTransform(Vector3 targetCamPos)
    {
        // Apply camera bounds
        targetCamPos.x = Mathf.Clamp(targetCamPos.x, minX, maxX);

        if (targetCamPos.y < 5.5f)
        {
            targetCamPos.y = minY;
        }
        targetCamPos.y = Mathf.Clamp(targetCamPos.y, minY, maxY);

        // Smooth camera movement using SmoothDamp for better feel
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetCamPos,
            ref velocity,
            smoothTime
        );
    }
}