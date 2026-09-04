using Unity.Mathematics;
using UnityEngine;

public class Read : MonoBehaviour
{
    [SerializeField] public Transform parent;
    [SerializeField] private float smoothSpeed = 8f;
    public bool ispickedUp = false;

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Collider col;
    private Rigidbody rb;

    public Vector3 pickupLocalPosition;
    private Quaternion readRotation = Quaternion.Euler(0, -180, 0);

    private void Awake()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
        initialPosition = transform.position;
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (ispickedUp)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pickupLocalPosition, smoothSpeed * Time.deltaTime);

            // Compensate for parent scale changes (e.g., crouching)
            Vector3 parentScale = parent.lossyScale;
            transform.localScale = new Vector3(
                initialScale.x / parentScale.x,
                initialScale.y / parentScale.y,
                initialScale.z / parentScale.z
            );
        }
    }

    public void ReadObject()
    {
        if (!ispickedUp)
        {
            ispickedUp = true;

            if (col != null) col.enabled = false;
            if (rb != null) rb.isKinematic = true; // stop physics from fighting the transform while held

            transform.SetParent(parent, false);
            transform.localRotation = readRotation;

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

            transform.SetParent(null, true);

            transform.position = initialPosition;
            transform.rotation = initialRotation;
            transform.localScale = initialScale;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.position = initialPosition;
                rb.rotation = initialRotation;
                Physics.SyncTransforms();
                Debug.Log("Rigidbody position after set: " + rb.position);
            }

            if (col != null) col.enabled = true;
        }
    }
}