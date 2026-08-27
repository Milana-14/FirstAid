using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot; // the object your orbit camera pitches on

    [Header("Sensitivity")]
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float minPitch = -40f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Invert")]
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    private void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = cameraPivot != null ? cameraPivot.localEulerAngles.x : 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Requires an action named "Look" in your input map
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void Update()
    {
        float xSign = invertX ? -1f : 1f;
        float ySign = invertY ? 1f : -1f; // default (not inverted): mouse up = look up

        yaw += lookInput.x * lookSensitivity * xSign;
        pitch += lookInput.y * lookSensitivity * ySign;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Yaw rotates the whole player (so movement direction turns with the camera)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Pitch only tilts the camera pivot, not the player body
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}