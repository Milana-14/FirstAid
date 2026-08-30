using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] public Transform parent;
    [SerializeField] private float smoothSpeed = 8f;
    public bool ispickedUp = false;

    private Vector3 lastPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Collider col;
    private Rigidbody rb;

    public Vector3 pickupLocalPosition; 

    private void Awake()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (ispickedUp)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pickupLocalPosition, smoothSpeed * Time.deltaTime);
        }
    }

    public void PickUpObject()
    {
        if (!ispickedUp)
        {
            ispickedUp = true;

            if (col != null) col.enabled = false;
            if (rb != null) rb.isKinematic = true; // stop physics from fighting the transform while held

            transform.SetParent(parent, false);
            transform.localRotation = Quaternion.identity;

            // compensate for parent's scale so the object keeps its original world size
            Vector3 parentScale = parent.lossyScale;
            transform.localScale = new Vector3(
                initialScale.x / parentScale.x,
                initialScale.y / parentScale.y,
                initialScale.z / parentScale.z
            );
        }
        else
        {
            ispickedUp = false;

            // convert local pickup offset into world space, accounting for parent rotation/scale
            lastPosition = parent.TransformPoint(pickupLocalPosition);

            transform.SetParent(null, true); // preserve world position on unparent

            transform.position = lastPosition;
            transform.rotation = initialRotation;
            transform.localScale = initialScale;

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.position = lastPosition;
                rb.rotation = initialRotation;
                Physics.SyncTransforms();
                Debug.Log("Rigidbody position after set: " + rb.position);
            }

            if (col != null) col.enabled = true;
        }
    }
}