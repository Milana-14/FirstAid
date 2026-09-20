using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class InteractableObjects : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isOpen;
    
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void Interact(Transform obj)
    {
        isOpen = !isOpen;
        animator.Play(isOpen ? $"Open_{obj.name}" : $"Close_{obj.name}");
    }
}