using UnityEngine;

public sealed class InteractableObjects : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openState;
    [SerializeField] private string closeState;

    private bool isOpen;
    
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        isOpen = !isOpen;
        animator.Play(isOpen ? openState : closeState);
    }
}