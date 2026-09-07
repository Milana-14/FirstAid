using Mono.Cecil.Cil;
using NUnit.Framework;
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

    [Header("Placing")]
    [SerializeField] private string placeable;

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
            InteractableObjects parentInteractable = hit.transform.parent != null ? hit.transform.parent.GetComponent<InteractableObjects>() : null;
            Animator parentAnim = hit.transform.parent != null ? hit.transform.parent.GetComponent<Animator>() : null;

            if (parentInteractable != null && parentAnim != null && hit.transform.GetComponent<Read>() == null)
            {
                Transform parentObj = hit.transform.parent;
                isHandled = true;
                state = parentInteractable.isActivated ? "затвориш" : "отвориш";
                pointer.color = Color.lightGreen;
                sign.text = $"Натисни E, за да {state}";

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    // the parent of the hit object is the one holding Animator + InteractableObjects
                    Transform target = parentObj;
                    Animator anim = parentAnim;
                    InteractableObjects interactable = parentInteractable;

                    
                    interactable.Interact(target);
                }
            }
            else if (hit.collider.GetComponent<PickUp>() != null)
            {
                isHandled = true;
                pointer.color = Color.yellowGreen;

                if (!isHoldingL && !isHoldingR)
                {
                    sign.text = "Натисни E или Q, за да вземеш";
                }
                else if (isHoldingL && !isHoldingR)
                {
                    if (leftheldObject.transform.GetComponent<Read>() != null)
                    {
                        sign.text = "Натисни E, за да вземеш, или Q, за да върнеш";
                    }
                    else
                    {
                        sign.text = "Натисни E, за да вземеш, или Q, за да пуснеш";
                    }
                }
                else if (!isHoldingL && isHoldingR)
                {
                    if (rightheldObject.transform.GetComponent<Read>() != null)
                    {
                        sign.text = "Натисни Q, за да вземеш, или E, за да върнеш";
                    }
                    else
                    {
                        sign.text = "Натисни Q, за да вземеш, или E, за да пуснеш";
                    }
                }
                else
                {
                    sign.text = "Ръцете ти са пълни";
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
                        if (rightheldObject.transform.GetComponent<Read>() != null)
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
                        if (leftheldObject.transform.GetComponent<Read>() != null)
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
                    sign.text = "Натисни E или Q, за да четеш";
                }
                else if (isHoldingL && !isHoldingR)
                {
                    if (leftheldObject.transform.GetComponent<PickUp>() != null)
                    {
                        sign.text = "Натисни E, за да четеш, или Q, за да пуснеш";
                    }
                    else
                    {
                        sign.text = "Натисни E, за да четеш, или Q, за да върнеш";
                    }
                }
                else if (!isHoldingL && isHoldingR)
                {
                    if (rightheldObject.transform.GetComponent<Read>() != null)
                    {
                        sign.text = "Натисни Q, за да четеш, или E, за да върнеш";
                    }
                    else
                    {
                        sign.text = "Натисни Q, за да четеш, или E, за да пуснеш";
                    }
                }
                else
                {
                    sign.text = "Ръцете ти са пълни";
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
                        if (rightheldObject.transform.GetComponent<Read>() != null)
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
                        if (leftheldObject.transform.GetComponent<Read>() != null)
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
            else if(hit.transform.tag == placeable)
            {
                isHandled = true;
                pointer.color = Color.cyan;
                sign.text = "Натисни E, за да поставиш";

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    if (isHoldingR)
                    {
                        rightheldObject.GetComponent<PlaceDown>().Place(hit.transform);
                        isHoldingR = false;
                    }
                }
            }
            else if (isHandled == false)
            {
                bool leftIsRead = false;
                bool rightIsRead = false;

                if (leftheldObject != null)
                {
                    leftIsRead = leftheldObject.transform.GetComponent<Read>() != null;
                }
                if (rightheldObject != null)
                {
                    rightIsRead = rightheldObject.transform.GetComponent<Read>() != null;
                }

                pointer.color = Color.white;
                if (!isHoldingL && !isHoldingR)
                {
                    sign.text = string.Empty;
                }
                else if (isHoldingL && isHoldingR)
                {
                    if (leftIsRead && rightIsRead)
                    {
                        sign.text = "Натисни E или Q, за да върнеш";
                    }
                    else if (leftIsRead && !rightIsRead)
                    {
                        sign.text = "Натисни Q, за да върнеш, или E, за да пуснеш";
                    }
                    else if (!leftIsRead && rightIsRead)
                    {
                        sign.text = "Натисни Q, за да пуснеш, или E, за да върнеш";
                    }
                    else
                    {
                        sign.text = "Натисни E или Q, за да пуснеш";
                    }

                }
                else if (isHoldingL && !isHoldingR)
                {
                    if (leftIsRead)
                    {
                        sign.text = "Натисни Q, за да върнеш";
                    }
                    else
                    {
                        sign.text = "Натисни Q, за да пуснеш";
                    }
                }
                else if (!isHoldingL && isHoldingR)
                {
                    if (rightIsRead)
                    {
                        sign.text = "Натисни E, за да върнеш";
                    }
                    else
                    {
                        sign.text = "Натисни E, за да пуснеш";
                    }
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
            bool leftIsRead = false;
            bool rightIsRead = false;

            if (leftheldObject != null)
            {
                leftIsRead = leftheldObject.transform.GetComponent<Read>() != null;
            }
            if (rightheldObject != null)
            {
                rightIsRead = rightheldObject.transform.GetComponent<Read>() != null;
            }

            pointer.color = Color.white;
            if (!isHoldingL && !isHoldingR)
            {
                sign.text = string.Empty;
            }
            else if (isHoldingL && isHoldingR)
            {
                if (leftIsRead && rightIsRead)
                {
                    sign.text = "Натисни E или Q, за да върнеш";
                }
                else if (leftIsRead && !rightIsRead)
                {
                    sign.text = "Натисни Q, за да върнеш, или E, за да пуснеш";
                }
                else if (!leftIsRead && rightIsRead)
                {
                    sign.text = "Натисни Q, за да пуснеш, или E, за да върнеш";
                }
                else
                {
                    sign.text = "Натисни E или Q, за да пуснеш";
                }

            }
            else if (isHoldingL && !isHoldingR)
            {
                if (leftIsRead)
                {
                    sign.text = "Натисни Q, за да върнеш";
                }
                else
                {
                    sign.text = "Натисни Q, за да пуснеш";
                }
            }
            else if (!isHoldingL && isHoldingR)
            {
                if (rightIsRead)
                {
                    sign.text = "Натисни E, за да върнеш";
                }
                else
                {
                    sign.text = "Натисни E, за да пуснеш";
                }
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