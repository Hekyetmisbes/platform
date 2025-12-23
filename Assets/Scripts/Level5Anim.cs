using UnityEngine;

public class Level5Anim : MonoBehaviour
{
    [SerializeField] 
    private PlayerController playerController;

    private bool isFadingOut = true;
    private SpriteRenderer playerRenderer;

    void Awake()
    {
        if (playerController != null)
        {
            playerRenderer = playerController.GetComponent<SpriteRenderer>();
        }
    }

    void AnimationEvent()
    {
        if (playerRenderer == null) return;

        Color color = playerRenderer.color;
        if (isFadingOut)
        {
            color.a = Mathf.Clamp(color.a - 0.05f, 0f, 1f);
            if (color.a <= 0f)
            {
                isFadingOut = false;
            }
        }
        else
        {
            color.a = Mathf.Clamp(color.a + 0.01f, 0f, 1f);
            if (color.a >= 1f)
            {
                isFadingOut = true;
            }
        }
        playerRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController == null || playerRenderer == null) return;

        if(!playerController.IsDead || !playerController.IsFinish)
        {
            AnimationEvent();
        }
    }
}
