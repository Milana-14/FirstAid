using UnityEngine;

public class PlaceDown : MonoBehaviour
{
    private Transform placeableObj;
    bool isPlaced = false;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private void Awake()
    {
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if(transform.GetComponent<PickUp>().enabled == true)
        {
            isPlaced = false;
        }
    }

    public void Place(Transform placeableObj)
    {
        if (placeableObj != null)
        {
            if (!isPlaced)
            {
                transform.GetComponent<PickUp>().PickUpObject();
                transform.localScale = new Vector3(1, 1, 1);
                transform.SetParent(placeableObj, true);
                transform.localRotation = initialRotation;
                transform.position = new Vector3(0, -0.002f, 0);
                transform.GetComponent<Rigidbody>().isKinematic = true;
                isPlaced = true;
            }
        }
    }
}
