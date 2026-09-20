using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class PlayerInteraction : MonoBehaviour
{
    private enum Hand
    {
        Left,
        Right
    }

    [Header("Ray")]
    [SerializeField] private Transform rayHolder;
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private LayerMask collisionLayers = Physics.DefaultRaycastLayers;

    [Header("UX")]
    [SerializeField] private Image pointer;
    [SerializeField] private TextMeshProUGUI sign;

    [Header("Hands")]
    [SerializeField] private Vector3 rightHandPlacement = new(0.791f, 0.51f, 1.25f);
    [SerializeField] private Vector3 leftHandPlacement = new(-0.791f, 0.51f, 1.18f);
    [SerializeField] private Vector3 rightReadPlacement = new(0.537f, 0.796f, 0.77f);
    [SerializeField] private Vector3 leftReadPlacement = new(-0.537f, 0.796f, 0.77f);

    [Header("Pointer Colors")]
    [SerializeField] private Color interactColor = Color.lightGreen;
    [SerializeField] private Color pickUpColor = Color.yellowGreen;
    [SerializeField] private Color readColor = Color.lawnGreen;
    [SerializeField] private Color placeColor = Color.cyan;

    private Transform leftHeldObject;
    private Transform rightHeldObject;

    private string currentPromptKey;

    private void Update()
    {
        if (rayHolder == null) return;

        if (Physics.Raycast(rayHolder.position, rayHolder.forward, out RaycastHit hit, rayLength, collisionLayers))
            HandleTarget(hit);
        else 
            HandleNoTarget();
    }

    private void HandleTarget(RaycastHit hit)
    {
        IInteractionHintProvider hintProvider = hit.transform.GetComponent<IInteractionHintProvider>() 
                                                ?? hit.transform.GetComponentInParent<IInteractionHintProvider>();

        if (hintProvider != null && !string.IsNullOrEmpty(hintProvider.HintKey))
        {
            ShowPrompt(hintProvider.HintKey);
        }
        
        InteractableObjects interactable = hit.transform.GetComponent<InteractableObjects>();
        bool isReadable = hit.transform.GetComponent<Read>() != null;

        if (interactable == null && !isReadable)
        {
            interactable = hit.transform.GetComponentInParent<InteractableObjects>();
        }

        if (interactable != null)
        {
            HandleInteractable(interactable);
            return;
        }

        PickUp pickUp = hit.transform.GetComponent<PickUp>();

        if (pickUp == null) pickUp = hit.transform.GetComponentInParent<PickUp>();

        if (pickUp != null)
        {
            HandlePickUpTarget(pickUp);
            return;
        }

        Read read = hit.transform.GetComponent<Read>();

        if (read == null) read = hit.transform.GetComponentInParent<Read>();

        if (read != null)
        {
            HandleReadTarget(read);
            return;
        }

        PlaceableObj placeable = hit.transform.GetComponent<PlaceableObj>();

        if (placeable == null) placeable = hit.transform.GetComponentInParent<PlaceableObj>();

        if (placeable != null)
        {
            HandlePlaceableTarget(placeable);
            return;
        }

        HidePrompt();
        SetPointerColor(Color.white);
    }

    private void HandleInteractable(InteractableObjects interactable, string customPromptKey = null)
    {
        SetPointerColor(interactColor);
        
        string promptToKey = !string.IsNullOrEmpty(customPromptKey) ? customPromptKey : "Prompt_Interact";
        ShowPrompt(promptToKey);

        if (Keyboard.current.eKey.wasPressedThisFrame) interactable.Interact(interactable.transform);
    }

    private void HandlePickUpTarget(PickUp target)
    {
        SetPointerColor(pickUpColor);

        bool rightHoldingTarget = rightHeldObject == target.transform;
        bool leftHoldingTarget = leftHeldObject == target.transform;

        if (rightHoldingTarget)
        {
            ShowPrompt("Prompt_Drop_Right");

            if (Keyboard.current.eKey.wasPressedThisFrame) DropHeldObject(Hand.Right);

            return;
        }

        if (leftHoldingTarget)
        {
            ShowPrompt("Prompt_Drop_Left");

            if (Keyboard.current.qKey.wasPressedThisFrame) DropHeldObject(Hand.Left);

            return;
        }

        bool rightFree = rightHeldObject == null;
        bool leftFree = leftHeldObject == null;

        if (rightFree && leftFree)
        {
            ShowPrompt("Prompt_PickUp_TwoHands");

            if (Keyboard.current.eKey.wasPressedThisFrame) PickUpObject(target, Hand.Right);

            if (Keyboard.current.qKey.wasPressedThisFrame) PickUpObject(target, Hand.Left);

            return;
        }

        if (rightFree)
        {
            ShowPrompt("Prompt_PickUp_Right");

            if (Keyboard.current.eKey.wasPressedThisFrame) PickUpObject(target, Hand.Right);

            return;
        }

        if (leftFree)
        {
            ShowPrompt("Prompt_PickUp_Left");

            if (Keyboard.current.qKey.wasPressedThisFrame) PickUpObject(target, Hand.Left);

            return;
        }

        ShowPrompt("Prompt_HandsFull");
    }

    private void HandleReadTarget(Read target)
    {
        SetPointerColor(readColor);

        bool rightHoldingTarget = rightHeldObject == target.transform;
        bool leftHoldingTarget = leftHeldObject == target.transform;

        if (rightHoldingTarget)
        {
            ShowPrompt("Prompt_Return_Right");

            if (Keyboard.current.eKey.wasPressedThisFrame) ReturnReadObject(Hand.Right);

            return;
        }

        if (leftHoldingTarget)
        {
            ShowPrompt("Prompt_Return_Left");

            if (Keyboard.current.qKey.wasPressedThisFrame) ReturnReadObject(Hand.Left);

            return;
        }

        bool rightFree = rightHeldObject == null;
        bool leftFree = leftHeldObject == null;

        if (rightFree && leftFree)
        {
            ShowPrompt("Prompt_Read_TwoHands");

            if (Keyboard.current.eKey.wasPressedThisFrame) ReadObject(target, Hand.Right);
            if (Keyboard.current.qKey.wasPressedThisFrame) ReadObject(target, Hand.Left);

            return;
        }

        if (rightFree)
        {
            ShowPrompt("Prompt_Read_Right");

            if (Keyboard.current.eKey.wasPressedThisFrame) ReadObject(target, Hand.Right);

            return;
        }

        if (leftFree)
        {
            ShowPrompt("Prompt_Read_Left");

            if (Keyboard.current.qKey.wasPressedThisFrame) ReadObject(target, Hand.Left);

            return;
        }

        ShowPrompt("Prompt_HandsFull");
    }

    private void HandlePlaceableTarget(PlaceableObj placeable)
    {
        SetPointerColor(placeColor);

        bool rightHolding = rightHeldObject != null;
        bool leftHolding = leftHeldObject != null;

        if (!placeable.HasFreePosition)
        {
            ShowPrompt("Prompt_NoRoom", placeable.CurrentCount, placeable.Capacity);
            return;
        }

        bool rightCanPlace = rightHolding && rightHeldObject.GetComponent<PlaceDown>() != null;
        bool leftCanPlace = leftHolding && leftHeldObject.GetComponent<PlaceDown>() != null;
        bool rightIsRead = rightHolding && rightHeldObject.GetComponent<Read>() != null;
        bool leftIsRead = leftHolding && leftHeldObject.GetComponent<Read>() != null;

        if (rightCanPlace && leftCanPlace)
        {
            ShowPrompt("Prompt_Place_TwoHands");

            if (Keyboard.current.eKey.wasPressedThisFrame) PlaceHeldObject(placeable, Hand.Right);

            if (Keyboard.current.qKey.wasPressedThisFrame) PlaceHeldObject(placeable, Hand.Left);

            return;
        }

        if (rightCanPlace)
        {
            ShowPrompt("Prompt_Place_Right");

            if (Keyboard.current.eKey.wasPressedThisFrame) PlaceHeldObject(placeable, Hand.Right);

            return;
        }

        if (leftCanPlace)
        {
            ShowPrompt("Prompt_Place_Left");

            if (Keyboard.current.qKey.wasPressedThisFrame) PlaceHeldObject(placeable, Hand.Left);

            return;
        }

        if (rightIsRead)
        {
            ShowPrompt("Prompt_Return_Right");

            if (Keyboard.current.eKey.wasPressedThisFrame) ReturnReadObject(Hand.Right);

            return;
        }

        if (leftIsRead)
        {
            ShowPrompt("Prompt_Return_Left");

            if (Keyboard.current.qKey.wasPressedThisFrame) ReturnReadObject(Hand.Left);

            return;
        }

        HidePrompt();
    }

    private void HandleNoTarget()
    {
        SetPointerColor(Color.white);

        bool rightHolding = rightHeldObject != null;
        bool leftHolding = leftHeldObject != null;

        if (!rightHolding && !leftHolding)
        {
            HidePrompt();
            return;
        }

        if (rightHolding && leftHolding)
        {
            bool rightIsRead = IsReadObject(rightHeldObject);
            bool leftIsRead = IsReadObject(leftHeldObject);

            if (rightIsRead && leftIsRead) ShowPrompt("Prompt_Return_TwoHands");
            else if (rightIsRead) ShowPrompt("Prompt_Return_Right_Drop_Left");
            else if (leftIsRead) ShowPrompt("Prompt_Drop_Right_Return_Left");
            else ShowPrompt("Prompt_Drop_TwoHands");
        }
        else if (rightHolding)
        {
            if (IsReadObject(rightHeldObject)) ShowPrompt("Prompt_Return_Right");
            else ShowPrompt("Prompt_Drop_Right");
        }
        else
        {
            if (IsReadObject(leftHeldObject)) ShowPrompt("Prompt_Return_Left");
            else ShowPrompt("Prompt_Drop_Left");
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && rightHolding) DropOrReturn(Hand.Right);
        if (Keyboard.current.qKey.wasPressedThisFrame && leftHolding) DropOrReturn(Hand.Left);
    }

    private void PickUpObject(PickUp pickUp, Hand hand)
    {
        if (GetHeldObject(hand) != null) return;

        pickUp.SetPickupPosition(GetHandPosition(hand));
        pickUp.PickUpObject();

        SetHeldObject(hand, pickUp.transform);
    }

    private void ReadObject(Read read, Hand hand)
    {
        if (GetHeldObject(hand) != null) return;

        read.SetReadPosition(GetReadPosition(hand));
        read.ReadObject();

        SetHeldObject(hand, read.transform);
    }

    private void PlaceHeldObject(PlaceableObj placeable, Hand hand)
    {
        Transform heldObject = GetHeldObject(hand);

        if (heldObject == null) return;

        PlaceDown placeDown = heldObject.GetComponent<PlaceDown>();

        if (placeDown == null) return;

        placeDown.Place(placeable.transform);
        ClearHeldObject(hand);
    }

    private void ReturnReadObject(Hand hand)
    {
        Transform heldObject = GetHeldObject(hand);

        if (heldObject == null) return;

        Read read = heldObject.GetComponent<Read>();

        if (read == null) return;

        read.ReadObject();
        ClearHeldObject(hand);
    }

    private void DropOrReturn(Hand hand)
    {
        Transform heldObject = GetHeldObject(hand);

        if (heldObject == null) return;

        if (IsReadObject(heldObject)) ReturnReadObject(hand);
        else DropHeldObject(hand);
    }

    private void DropHeldObject(Hand hand)
    {
        Transform heldObject = GetHeldObject(hand);

        if (heldObject == null) return;

        PickUp pickUp = heldObject.GetComponent<PickUp>();

        if (pickUp != null) pickUp.Drop();

        ClearHeldObject(hand);
    }

    private bool IsReadObject(Transform target)
    {
        Read read = target.GetComponent<Read>();
        return read != null && read.IsReading;
    }

    private Transform GetHeldObject(Hand hand)
    {
        return hand == Hand.Right ? rightHeldObject : leftHeldObject;
    }

    private void SetHeldObject(Hand hand, Transform target)
    {
        if (hand == Hand.Right) rightHeldObject = target;
        else leftHeldObject = target;
    }

    private void ClearHeldObject(Hand hand)
    {
        if (hand == Hand.Right) rightHeldObject = null;
        else leftHeldObject = null;
    }

    private Vector3 GetHandPosition(Hand hand)
    {
        return hand == Hand.Right ? rightHandPlacement : leftHandPlacement;
    }

    private Vector3 GetReadPosition(Hand hand)
    {
        return hand == Hand.Right ? rightReadPlacement : leftReadPlacement;
    }

    private void ShowPrompt(string key, params object[] args)
    {
        if (sign == null) return;
        if (key == currentPromptKey) return;

        currentPromptKey = key;

        LocalizationService.Instance.GetLocalizedStringWithArgs("UI_Table", key, args, localizedText => sign.text = localizedText);
    }

    private void HidePrompt()
    {
        if (sign == null) return;
        if (string.IsNullOrEmpty(currentPromptKey)) return;

        currentPromptKey = string.Empty;
        sign.text = string.Empty;
    }

    private void SetPointerColor(Color color)
    {
        if (pointer != null) pointer.color = color;
    }

    private void OnDrawGizmosSelected()
    {
        if (rayHolder == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayHolder.position, rayHolder.forward * rayLength);
    }
}