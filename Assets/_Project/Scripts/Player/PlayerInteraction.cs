using Mono.Cecil.Cil;
using TMPro;
using Unity.AI.Assistant.Agents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("RayHolder")]
    [SerializeField] private Transform rayHolder;

    [Header("Ray")]
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private LayerMask collisionLayers = Physics.DefaultRaycastLayers;

    [Header("UX")]
    [SerializeField] private Image pointer;
    [SerializeField] private TextMeshProUGUI sign;

    private bool isHoldingR = false;
    private bool isHoldingL = false;

    private Transform leftheldObject;
    private Transform rightheldObject;

    private Vector3 rightHandPlacement = new(0.791f, 0.51f, 1.25f);
    private Vector3 leftHandPlacement = new(-0.791f, 0.51f, 1.18f);

    private Vector3 rightReadPlacement = new(0.537f, 0.796f, 0.77f);
    private Vector3 leftReadPlacement = new(-0.537f, 0.796f, 0.77f);

    private string state;

    private bool isHandled;

    void Update()
    {
        if (rayHolder == null)
        {
            return;
        }

        isHandled = false;

        Ray ray = new Ray(rayHolder.position, rayHolder.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, collisionLayers))
        {
            if (hit.transform.childCount != 0)
            {
                if (hit.transform.GetChild(0).GetComponent<InteractableObjects>() != null && hit.transform.GetChild(0).GetComponent<Animator>() != null)
                {
                    isHandled = true;
                    state = hit.transform.GetChild(0).GetComponent<InteractableObjects>().isActivated ? "Close" : "Open";
                    pointer.color = Color.lightGreen;
                    sign.text = $"Press E to {state}";

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

                        if(hit.collider.name.Contains("Door"))
                        {
                            blockcol.enabled = interactable.isActivated;
                        }
                        interactable.Interact(target);
                    }
                }
            }
            else if (hit.collider.GetComponent<PickUp>() != null)
            {
                isHandled = true;
                pointer.color = Color.yellowGreen;

                if (!isHoldingL && !isHoldingR)
                {
                    sign.text = "Press E or Q to grab";
                }
                else if (isHoldingL && !isHoldingR)
                {
                    sign.text = "Press E to grab or Q to put back";
                }
                else if (!isHoldingL && isHoldingR)
                {
                    sign.text = "Press Q to grab or E to put back";
                }
                else
                {
                    sign.text = "Your hands are full";
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
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
                        if(rightheldObject.transform.GetComponent<Read>() != null)
                        {
                            rightheldObject.GetComponent<Read>().pickupLocalPosition = rightReadPlacement;
                            rightheldObject.GetComponent<Read>().ReadObject();
                        }
                        else
                        {
                            rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                            rightheldObject.GetComponent<PickUp>().PickUpObject();
                        }
                        isHoldingR = false;
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

                if (Keyboard.current.qKey.wasPressedThisFrame)
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
                        if(leftheldObject.transform.GetComponent<Read>() != null)
                        {
                            leftheldObject.GetComponent<Read>().pickupLocalPosition = leftReadPlacement;
                            leftheldObject.GetComponent<Read>().ReadObject();
                        }
                        else
                        {
                            leftheldObject.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                            leftheldObject.GetComponent<PickUp>().PickUpObject();
                        }
                        isHoldingL = false;
                    }
                }
            }
            else if (hit.collider.GetComponent<Read>() != null)
            {
                isHandled = true;
                pointer.color = Color.lawnGreen;

                if (!isHoldingL && !isHoldingR)
                {
                    sign.text = "Press E or Q to read";
                }
                else if (isHoldingL && !isHoldingR)
                {
                    sign.text = "Press E to read or Q to put back";
                }
                else if (!isHoldingL && isHoldingR)
                {
                    sign.text = "Press Q to read or E to put back";
                }
                else
                {
                    sign.text = "Your hands are full";
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    if (!isHoldingR)
                    {
                        hit.collider.GetComponent<Read>().pickupLocalPosition = rightReadPlacement;
                        hit.collider.GetComponent<Read>().ReadObject();
                        isHoldingR = true;
                        rightheldObject = hit.collider.transform;
                    }
                    else
                    {
                        if(rightheldObject.transform.GetComponent<Read>() != null)
                        {
                            rightheldObject.GetComponent<Read>().pickupLocalPosition = rightReadPlacement;
                            rightheldObject.GetComponent<Read>().ReadObject();
                        }
                        else
                        {
                            rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                            rightheldObject.GetComponent<PickUp>().PickUpObject();
                        }
                        isHoldingR = false;
                    }
                }

                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    if (!isHoldingL)
                    {
                        hit.collider.GetComponent<Read>().pickupLocalPosition = leftReadPlacement;
                        hit.collider.GetComponent<Read>().ReadObject();
                        isHoldingL = true;
                        leftheldObject = hit.collider.transform;
                    }
                    else
                    {
                        if(leftheldObject.transform.GetComponent<Read>() != null)
                        {
                            leftheldObject.GetComponent<Read>().pickupLocalPosition = leftReadPlacement;
                            leftheldObject.GetComponent<Read>().ReadObject();
                        }
                        else
                        {
                            leftheldObject.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                            leftheldObject.GetComponent<PickUp>().PickUpObject();
                        }
                        isHoldingL = false;
                    }
                }

            }
            else if (isHandled == false)
            {
                pointer.color = Color.white;
                if (!isHoldingL && !isHoldingR)
                {
                    sign.text = string.Empty;
                }
                else if (isHoldingL && isHoldingR)
                {
                    sign.text = "Press E or Q to put back";
                }
                else if (isHoldingL && !isHoldingR)
                {
                    sign.text = "Press Q to put back";
                }
                else if (!isHoldingL && isHoldingR)
                {
                    sign.text = "Press E to put back";
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    if (isHoldingR && rightheldObject.transform.GetComponent<Read>() == null)
                    {
                        rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                        rightheldObject.GetComponent<PickUp>().PickUpObject();
                        isHoldingR = false;
                    }
                    else if (isHoldingR && rightheldObject.transform.GetComponent<Read>() != null)
                    {
                        rightheldObject.GetComponent<Read>().pickupLocalPosition = rightReadPlacement;
                        rightheldObject.GetComponent<Read>().ReadObject();
                        isHoldingR = false;
                    }
                }
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    if (isHoldingL && leftheldObject.transform.GetComponent<Read>() == null)
                    {
                        leftheldObject.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                        leftheldObject.GetComponent<PickUp>().PickUpObject();
                        isHoldingL = false;
                    }
                    else if (isHoldingL && leftheldObject.transform.GetComponent<Read>() != null)
                    {
                        leftheldObject.GetComponent<Read>().pickupLocalPosition = leftReadPlacement;
                        leftheldObject.GetComponent<Read>().ReadObject();
                        isHoldingL = false;
                    }
                }
            }
        }
        else
        {
            pointer.color = Color.white;
            if(!isHoldingL && !isHoldingR)
            {
                sign.text = string.Empty;
            }
            else if (isHoldingL && isHoldingR)
            {
                sign.text = "Press E or Q to put back";
            }
            else if (isHoldingL && !isHoldingR)
            {
                sign.text = "Press Q to put back";
            }
            else if (!isHoldingL && isHoldingR)
            {
                sign.text = "Press E to put back";
            }
    
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (isHoldingR && rightheldObject.transform.GetComponent<Read>() == null)
                {
                    rightheldObject.GetComponent<PickUp>().pickupLocalPosition = rightHandPlacement;
                    rightheldObject.GetComponent<PickUp>().PickUpObject();
                    isHoldingR = false;
                }
                else if (isHoldingR && rightheldObject.transform.GetComponent<Read>() != null)
                {
                    rightheldObject.GetComponent<Read>().pickupLocalPosition = rightReadPlacement;
                    rightheldObject.GetComponent<Read>().ReadObject();
                    isHoldingR = false;
                }
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (isHoldingL && leftheldObject.transform.GetComponent<Read>() == null)
                {
                    leftheldObject.GetComponent<PickUp>().pickupLocalPosition = leftHandPlacement;
                    leftheldObject.GetComponent<PickUp>().PickUpObject();
                    isHoldingL = false;
                }
                else if (isHoldingL && leftheldObject.transform.GetComponent<Read>() != null)
                {
                    leftheldObject.GetComponent<Read>().pickupLocalPosition = leftReadPlacement;
                    leftheldObject.GetComponent<Read>().ReadObject();
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