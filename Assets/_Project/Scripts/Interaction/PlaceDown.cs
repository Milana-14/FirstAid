using System.Collections.Generic;
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
            if (!isPlaced && placeableObj.GetComponent<PlaceableObj>() != null && !placeableObj.GetComponent<PlaceableObj>().full)
            {
                PickUp pickupComponent = transform.GetComponent<PickUp>();

                if (pickupComponent.ispickedUp)
                {
                    pickupComponent.PickUpObject();
                }

                transform.SetParent(placeableObj, false);
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
                transform.localPosition = placeableObj.GetComponent<PlaceableObj>().getPPos();

                transform.GetComponent<Rigidbody>().isKinematic = true;
                isPlaced = true;

            }
        }
    }
}
