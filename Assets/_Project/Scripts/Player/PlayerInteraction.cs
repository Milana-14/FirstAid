using Mono.Cecil.Cil;
using Unity.AI.Assistant.Agents;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("RayHolder")]
    [SerializeField] private Transform rayHolder;

    [Header("Ray")]
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private LayerMask collisionLayers = Physics.DefaultRaycastLayers;

    private bool isHolding = false;

    private Transform heldObject;

    void Update()
    {
        if (rayHolder == null)
        {
            return;
        }

        Ray ray = new Ray(rayHolder.position, rayHolder.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, collisionLayers))
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Transform target = hit.collider.transform;
                Animator anim = target.GetComponent<Animator>();
                InteractableObjects interactable = target.GetComponent<InteractableObjects>();

                // if the hit object itself doesn't have what we need, try its first child
                if (anim == null || interactable == null)
                {
                    if (hit.collider.transform.childCount == 0)
                    {
                        Debug.LogWarning(hit.collider.name + " has no children and no Animator/InteractableObjects on itself.");
                        return;
                    }

                    target = hit.collider.transform.GetChild(0);
                    anim = target.GetComponent<Animator>();
                    interactable = target.GetComponent<InteractableObjects>();

                    if (anim == null)
                    {
                        Debug.LogWarning("No Animator found on " + hit.collider.name + " or its first child.");
                        return;
                    }

                    if (interactable == null)
                    {
                        Debug.LogWarning("No InteractableObjects found on " + hit.collider.name + " or its first child.");
                        return;
                    }
                }

                Collider blockcol = null;
                Collider[] allColliders = hit.collider.GetComponents<Collider>();

                foreach (Collider col in allColliders)
                {
                    if (!col.isTrigger) // the solid one, not the detection trigger
                    {
                        blockcol = col;
                        break;
                    }
                }

                if (blockcol == null)
                {
                    Debug.LogWarning("No blocking collider found on " + hit.collider.name);
                    return;
                }

                blockcol.enabled = interactable.isActivated;
                interactable.Interact(target);
            }

            if(Keyboard.current.gKey.wasPressedThisFrame)
            {
                if(!isHolding)
                {
                    hit.collider.GetComponent<PickUp>().PickUpObject();
                    isHolding = true;
                    heldObject = hit.collider.transform;
                } 
                else
                {
                    heldObject.GetComponent<PickUp>().PickUpObject();
                    isHolding= false;
                }
            }
        }
        else
        {
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                if(isHolding)
                {
                    heldObject.GetComponent<PickUp>().PickUpObject();
                    isHolding = false;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (rayHolder == null)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayHolder.position, rayHolder.forward * rayLength);
    }
}