using UnityEngine;

public class PlaceDown : MonoBehaviour
{
    private Transform placeableObj;
    bool isPlaced = false;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private void Awake()
    {
        //initialRotation = transform.rotation;
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

                transform.SetParent(placeableObj, false);
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
                transform.localPosition = new Vector3(0, -0.00001f, 0);

                transform.GetComponent<Rigidbody>().isKinematic = true;
                isPlaced = true;
            }
        }
    }
}
