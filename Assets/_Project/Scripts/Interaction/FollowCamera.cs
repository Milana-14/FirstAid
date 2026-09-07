using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraHolder;

    private void Awake()
    {
        if (cameraHolder == null)
        {
            Debug.LogWarning("Camera holder reference is missing.");
            return;
        }
    }

    void LateUpdate()
    {
        if (cameraHolder == null) return;

        transform.rotation = cameraHolder.rotation;
    }
}