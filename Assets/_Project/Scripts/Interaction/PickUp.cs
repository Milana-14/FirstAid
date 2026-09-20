using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PickUp : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 pickupLocalPosition;

    private string DefaultParentName = "Player";

    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Collider col;
    private Rigidbody rb;

    public bool IsPickedUp { get; private set; }
    
    private void Awake()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;

        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if(parent == null)
        {
            GameObject parentObject = GameObject.Find(DefaultParentName);
            if (parentObject != null) parent = parentObject.transform;
        }
    }
    
    private void Update()
    {
        if (!IsPickedUp) return;

        transform.localPosition = Vector3.Lerp(transform.localPosition, pickupLocalPosition, smoothSpeed * Time.deltaTime);
        Vector3 parentScale = parent.lossyScale;
        transform.localScale = new Vector3(initialScale.x / parentScale.x, initialScale.y / parentScale.y, initialScale.z / parentScale.z);
    }

    public void PickUpObject()
    {
        if (IsPickedUp) return;
        IsPickedUp = true;
        
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        transform.SetParent(parent, false);
        transform.localRotation = Quaternion.identity;

        Vector3 parentScale = parent.lossyScale;

        transform.localScale = new Vector3(initialScale.x / parentScale.x, initialScale.y / parentScale.y, initialScale.z / parentScale.z);
    }

    public void Drop()
    {
        if (!IsPickedUp) return;
        IsPickedUp = false;

        Vector3 worldPosition = parent.TransformPoint(pickupLocalPosition);

        transform.SetParent(null, true);
        transform.position = worldPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.position = worldPosition;
            rb.rotation = initialRotation;
        }

        if (col != null) col.enabled = true;
        Physics.SyncTransforms();
    }
    
    public void SetPickupPosition(Vector3 position)
    {
        pickupLocalPosition = position;
    }
}