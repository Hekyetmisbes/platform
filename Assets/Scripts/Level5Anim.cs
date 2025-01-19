using UnityEngine;

public class Level5Anim : MonoBehaviour
{
    [SerializeField] 
    private PlayerController playerController;

    private bool isFadingOut = true;

    void AnimationEvent()
    {
        Color color = playerController.GetComponent<SpriteRenderer>().color;
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
        playerController.GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerController.IsDead || !playerController.IsFinish)
        {
            AnimationEvent();
        }
    }
}
