using UnityEngine;

public class InteractableObjects : MonoBehaviour
{
    [Header("Interaction Settings")]

    public bool isActivated = false; 

    public void Interact(Transform obj)
    {
        Animator anim = obj.GetComponent<Animator>();

        if (!isActivated)
        {
            isActivated = true;
            anim.Play($"Open_{obj.name}");
        }
        else if (isActivated)
        {
            isActivated = false;
            anim.Play($"Close_{obj.name}");
        }

    }
}
