using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] public Transform parent;
    [SerializeField] private float smoothSpeed = 8f;
    public bool ispickedUp = false;

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Collider col;

    private readonly Vector3 pickupLocalPosition = new(0.791f, 0.39f, 1f);

    private void Awake()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
        col = GetComponent<Collider>();
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
            transform.SetParent(null, false);
            transform.position = initialPosition;
            transform.localScale = initialScale; // back to original world scale, no parent involved
            if (col != null) col.enabled = true;
        }
    }
}