using System.Collections;
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

    private Transform pin;

    private void Awake()
    {
        pin = transform.parent;
        initialScale = transform.lossyScale;
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

            transform.SetParent(null, true);
            pin.GetComponent<InteractableObjects>().Interact(pin);
            StartCoroutine(WaitForAnim(false)); // pin deactivates only after the wait, inside the coroutine

            if (col != null) col.enabled = false;
            if (rb != null) rb.isKinematic = true;

            transform.SetParent(parent, false);
            transform.localRotation = readRotation;

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

            pin.gameObject.SetActive(true);
            pin.GetComponent<InteractableObjects>().Interact(pin);
            StartCoroutine(WaitForAnim(true)); // already active, but harmless to reaffirm after wait

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
            }

            if (col != null) col.enabled = true;

            //transform.SetParent(pin, true);
        }
    }

    private IEnumerator WaitForAnim(bool activateAfter)
    {
        yield return new WaitForSeconds(0.25f); 

        pin.gameObject.SetActive(activateAfter);
    }
}