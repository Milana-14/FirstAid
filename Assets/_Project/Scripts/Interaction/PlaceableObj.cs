using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PickUp))]
[RequireComponent(typeof(Rigidbody))]
public sealed class PlaceableObj : MonoBehaviour
{
    PlaceableObj po;
    PickUp pu;
    [SerializeField] private readonly Dictionary<int, Vector3> objPositions = new()
    {
        { 0, new Vector3(0.00112f, -0.00001f, 0.00092f) },
        { 1, new Vector3(-0.00142f, -0.00001f, 0.00092f) },
        { 2, new Vector3(0.00112f, -0.00001f, -0.00158f) },
        { 3, new Vector3(-0.00142f, -0.00001f, -0.00158f) }
    };

    public int CurrentCount
    {
        get
        {
            int count = 0;

            foreach (Transform child in transform)
            {
                if (child.GetComponent<PlaceDown>() != null)
                    count++;
            }

            return count;
        }
    }
    public int Capacity => objPositions.Count;
    public bool HasFreePosition => CurrentCount < Capacity;

    private void Awake()
    {
        po = GetComponent<PlaceableObj>();
        pu = GetComponent<PickUp>();
    }

    private void Update()
    {
      SetState();
    }

    public bool TryGetPosition(out Vector3 position)
    {
        if (!HasFreePosition)
        {
            position = Vector3.zero;
            return false;
        }

        position = objPositions[CurrentCount];
        return true;
    }

    private void SetState()
    {
        if(!HasFreePosition)
        {
            pu.enabled = true;
            po.enabled = false;
        }
        else
        {
            pu.enabled = false;
            po.enabled = true;
        }
    }
}