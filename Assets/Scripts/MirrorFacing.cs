using UnityEngine;

// Attach this to a menu character or any object that should mirror another CharacterMovement's facing
public class MirrorFacing : MonoBehaviour
{
    [Tooltip("The CharacterMovement to mirror. If left empty, will try to find one on scene.")]
    public CharacterMovement source;

    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
        if (source == null)
        {
            // Use newer API when available to avoid obsolete warnings.
#if UNITY_2023_2_OR_NEWER
            source = UnityEngine.Object.FindFirstObjectByType<CharacterMovement>();
#else
            // Fall back for older Unity versions
            source = FindObjectOfType<CharacterMovement>();
#endif
        }

        if (source != null)
        {
            // initialize
            ApplyFacing(source.FacingRight);
            source.OnFacingChanged += ApplyFacing;
        }
    }

    void OnDestroy()
    {
        if (source != null)
        {
            source.OnFacingChanged -= ApplyFacing;
        }
    }

    void ApplyFacing(bool facingRight)
    {
        Vector3 s = baseScale;
        s.x = Mathf.Abs(s.x) * (facingRight ? 1f : -1f);
        transform.localScale = s;
    }
}
