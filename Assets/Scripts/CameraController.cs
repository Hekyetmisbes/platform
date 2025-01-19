using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Control camera position
    [SerializeField] Transform target;

    private float smoothing = 5f;

    [SerializeField] float minY = 0f;
    [SerializeField] float maxY = 0f;

    [SerializeField] float minX = 0f;
    [SerializeField] float maxX = 78.5f;

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

        if(target.transform.localScale.x < 0)
        {
            targetCamPos.x -= 5f;
        }
        
        ChangeCamTransform(targetCamPos);
    }

    void ChangeCamTransform(Vector3 targetCamPos)
    {
        targetCamPos.x = Mathf.Clamp(targetCamPos.x, minX, maxX); // Limit x value
        if(targetCamPos.y < 5.5f)
        {
            targetCamPos.y = minY;
        }
        targetCamPos.y = Mathf.Clamp(targetCamPos.y, minY, maxY); ; // Keep y value
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}