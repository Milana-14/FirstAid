using System.Collections.Generic;
using UnityEngine;

public class PlaceableObj : MonoBehaviour
{
    public readonly Dictionary<int, Vector3> objPositions = new Dictionary<int, Vector3>
    {
        {0, new Vector3(0.00112f, -0.00001f, 0.00092f)},
        {1, new Vector3(-0.00142f, -0.00001f, 0.00092f)},
        {2, new Vector3(0.00004f, -0.00001f, -0.000153f)}
    };
    public int children = 0;

    public bool full = false;

    private void Update()
    {
        children = transform.childCount;
        if (children >= 3)
        {
            full = true;
        }
    }

    public Vector3 getPPos()
    {
        if (!full)
        {
            Vector3 position = new Vector3(objPositions[children].x, objPositions[children].y, objPositions[children].z);
            return position;
        }
        else
        {
            Debug.LogWarning("PlaceableObj is full. Cannot get position for new object.");
            return Vector3.zero; 
        }
    }

}
