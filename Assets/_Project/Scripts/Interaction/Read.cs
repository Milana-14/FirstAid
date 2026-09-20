using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class Read : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Transform pin;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 pickupLocalPosition;
    [SerializeField] private Vector3 readRotation = new(0f, -180f, 0f);
    [SerializeField] private float pinAnimationDelay = 0.25f;

    private Transform initialParent;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;

    private Collider col;
    private Rigidbody rb;

    public bool IsReading { get; private set; }

    private void Awake()
    {
        initialParent = transform.parent;
        initialPosition = transform.position;
        initialScale = transform.lossyScale;
        initialRotation = transform.rotation;

        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (pin == null) pin = initialParent;
    }

    private void Update()
    {
        if (!IsReading) return;

        transform.localPosition = Vector3.Lerp(transform.localPosition, pickupLocalPosition, smoothSpeed * Time.deltaTime);
        Vector3 parentScale = parent.lossyScale;
        transform.localScale = new Vector3(initialScale.x / parentScale.x, initialScale.y / parentScale.y, initialScale.z / parentScale.z );
    }

    public void ReadObject()
    {
        if (!IsReading) StartReading();
        else StopReading();
    }

    private void StartReading()
    {
        IsReading = true;

        transform.SetParent(null, true);

        InteractableObjects interactable = pin.GetComponent<InteractableObjects>();

        if (interactable != null) interactable.Interact(pin);
        StartCoroutine(SetPinActiveAfterDelay(false));

        col.enabled = false;
        rb.isKinematic = true;

        transform.SetParent(parent, false);
        transform.localRotation = Quaternion.Euler(readRotation);

        ApplyParentScale();
    }

    private void StopReading()
    {
        IsReading = false;

        pin.gameObject.SetActive(true);

        InteractableObjects interactable = pin.GetComponent<InteractableObjects>();

        if (interactable != null) interactable.Interact(pin);

        StartCoroutine(SetPinActiveAfterDelay(true));

        transform.SetParent(null, true);

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        rb.isKinematic = true;
        rb.position = initialPosition;
        rb.rotation = initialRotation;

        col.enabled = true;

        Physics.SyncTransforms();
    }

    private void ApplyParentScale()
    {
        Vector3 parentScale = parent.lossyScale;
        transform.localScale = new Vector3(initialScale.x / parentScale.x, initialScale.y / parentScale.y, initialScale.z / parentScale.z );
    }

    private IEnumerator SetPinActiveAfterDelay(bool active)
    {
        yield return new WaitForSeconds(pinAnimationDelay);

        if (pin != null) pin.gameObject.SetActive(active);
    }
    
    public void SetReadPosition(Vector3 position)
    {
        pickupLocalPosition = position;
    }
}