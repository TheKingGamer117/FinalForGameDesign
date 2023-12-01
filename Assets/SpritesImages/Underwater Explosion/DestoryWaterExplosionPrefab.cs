using UnityEngine;

public class AutoDestroyAfterAnimation : MonoBehaviour
{
    private Animator animator;
    private float delay = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Assuming there is only one animation clip attached to this animator
        if (animator.runtimeAnimatorController.animationClips.Length > 0)
        {
            // Get the duration of the first animation clip
            delay = animator.runtimeAnimatorController.animationClips[0].length;
        }

        // Destroy the game object after the animation duration
        Destroy(gameObject, delay);
    }
}
