using UnityEngine;

[RequireComponent(typeof(PickUp))]
[RequireComponent(typeof(Rigidbody))]
public sealed class PlaceDown : MonoBehaviour
{
    private PickUp pickUp;
    private Rigidbody rb;

    private bool isPlaced;

    private void Awake()
    {
        pickUp = GetComponent<PickUp>();
        rb = GetComponent<Rigidbody>();
    }
    
    public void Place(Transform placeableObj)
    {
        if (placeableObj == null || isPlaced) return;

        PlaceableObj placeable = placeableObj.GetComponent<PlaceableObj>();

        if (placeable == null) return;
        if (!placeable.TryGetPosition(out Vector3 position)) return;
        if (!pickUp.IsPickedUp) return;

        pickUp.Drop();

        transform.SetParent(placeableObj, false);
        transform.localPosition = position;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        isPlaced = true;
    }

    public void RemoveFromPlace()
    {
        if (!isPlaced) return;

        isPlaced = false;
        transform.SetParent(null, true);
        rb.isKinematic = false;
    }
}