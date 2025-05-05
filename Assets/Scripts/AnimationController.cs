using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void PlayAnimation(string name)
    {
        animator.SetBool(name, true);    
    }

    public void StopAnimation(string name)
    {
        animator.SetBool(name, false);
    }

    public void TriggerAnimation(string name)
    {
        animator.SetTrigger(name);
    }
}