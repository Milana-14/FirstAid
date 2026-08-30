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

    private bool isHoldingR = false;
    private bool isHoldingL = false;

    private Transform leftheldObject;
    private Transform rightheldObject;

    private Vector3 rightHandPlacement = new(0.791f, 0.39f, 1.25f);
    private Vector3 leftHandPlacement = new(-0.791f, 0.39f, 1.18f);

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
                if (hit.transform.childCount != 0)
                {
                    if (hit.transform.GetChild(0).GetComponent<InteractableObjects>() != null && hit.transform.GetChild(0).GetComponent<Animator>() != null)
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
                }
                else if (hit.collider.GetComponent<PickUp>() != null)
                {
                    if (!isHoldingR)
                    {
                        hit.collider.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                        hit.collider.GetComponent<PickUp>().PickUpObject();
                        isHoldingR = true;
                        rightheldObject = hit.collider.transform;
                    }
                    else
                    {
                        rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                        rightheldObject.GetComponent<PickUp>().PickUpObject();
                        isHoldingR = false;
                    }
                }
            }

            /*if(Mouse.current.rightButton.wasPressedThisFrame)
            {
                if(!isHoldingR)
                {
                    hit.collider.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                    hit.collider.GetComponent<PickUp>().PickUpObject();
                    isHoldingR = true;
                    rightheldObject = hit.collider.transform;
                } 
                else
                {
                    rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                    rightheldObject.GetComponent<PickUp>().PickUpObject();
                    isHoldingR = false;
                }
            }*/
            if(Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (!isHoldingL)
                {
                    hit.collider.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                    hit.collider.GetComponent<PickUp>().PickUpObject();
                    isHoldingL = true;
                    leftheldObject = hit.collider.transform;
                }
                else
                {
                    leftheldObject.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                    leftheldObject.GetComponent<PickUp>().PickUpObject();
                    isHoldingL = false;
                }
            }

        }
        else
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if(isHoldingR)
                {
                    rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                    rightheldObject.GetComponent<PickUp>().PickUpObject();
                    isHoldingR = false;
                }
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (isHoldingL)
                {
                    leftheldObject.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                    leftheldObject.GetComponent<PickUp>().PickUpObject();
                    isHoldingL = false;
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