using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Control camera position
    public Transform target;

    public float smoothing = 5f;

    public float minX = 0f;
    public float maxX = 78.5f;

    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        targetCamPos.x = Mathf.Clamp(targetCamPos.x, minX, maxX); // Limit x value
        targetCamPos.y = 0f; // Keep y value
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}