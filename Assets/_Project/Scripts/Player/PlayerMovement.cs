using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 25f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Model Orientation")]
    [Tooltip("If your character visually walks backward from what you press, your model likely faces -Z. Check this to flip movement direction.")]
    [SerializeField] private bool modelFacesBackward = false;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 currentVelocity;
    private bool isGrounded;
    private bool jumpRequested;
    private bool isCrouch = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called automatically by PlayerInput (Behavior: Send Messages)
    // Requires an action named "Move" in your input map
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Requires an action named "Jump" in your input map
    public void OnJump(InputValue value)
    {
        Debug.Log($"Jump input received. isGrounded = {isGrounded}");

        if (value.isPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if(Keyboard.current.cKey.wasPressedThisFrame && !isCrouch)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1 , transform.localScale.z);
            transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
            isCrouch = true;
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame && isCrouch)
        {
            transform.localScale = new Vector3(transform.localScale.x, 2, transform.localScale.z);
            isCrouch = false;
        }

        if(Keyboard.current.shiftKey.IsPressed() && isGrounded)
        {
            acceleration = 50f;
            moveSpeed = 12f;
        }
        else
        {
            acceleration = 25f;
            moveSpeed = 6f;
        }
    }

    private void FixedUpdate()
    {
        // Convert 2D input into a direction relative to which way the player is facing
        float facingSign = modelFacesBackward ? -1f : 1f;
        Vector3 moveDir = ((transform.forward * moveInput.y) + (transform.right * moveInput.x)) * facingSign;
        Vector3 targetVelocity = moveDir * moveSpeed;

        // Smoothly approach the target speed instead of snapping to it
        Vector3 newVelocity = Vector3.MoveTowards(
            new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z),
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}