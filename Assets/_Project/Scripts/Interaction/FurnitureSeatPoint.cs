using UnityEngine;

public enum FurnitureType
{
    ChairOrArmchair,
    BedOrSofa
}

public sealed class FurnitureSeatPoint : MonoBehaviour
{
    [SerializeField] private FurnitureType furnitureType = FurnitureType.ChairOrArmchair;
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private float interactRadius = 2.2f;

    public FurnitureType Type => furnitureType;
    public Transform Anchor => anchorPoint != null ? anchorPoint : transform;
    public float InteractRadius => interactRadius;

    public bool CanSit => true;
    public bool CanLieDown => furnitureType == FurnitureType.BedOrSofa;

    private void OnDrawGizmos()
    {
        Transform target = Anchor;
        if (target == null) return;

        Gizmos.color = furnitureType == FurnitureType.BedOrSofa ? Color.cyan : Color.green;
        Gizmos.DrawWireSphere(target.position, 0.18f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(target.position, target.forward * 0.8f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(target.position, target.up * 0.6f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(target.position, target.right * 0.4f);
    }
}