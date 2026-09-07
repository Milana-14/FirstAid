using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 25f;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float sprintAcceleration = 50f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;

    [Header("Model Orientation")]
    [SerializeField] private bool modelFacesBackward = false;
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _isCrouch = false;

    private Vector3 _standingScale;
    private float _standingPositionY;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _standingScale = transform.localScale;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        Debug.Log($"Jump input received. isGrounded = {_isGrounded}");

        if (value.isPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (Keyboard.current.cKey.wasPressedThisFrame && !_isCrouch && _isGrounded)
        {
            _standingPositionY = transform.position.y;
            float heightDifference = _standingScale.y - crouchHeight;

            transform.localScale = new Vector3(_standingScale.x, crouchHeight, _standingScale.z);
            transform.position = new Vector3(transform.position.x, _standingPositionY - heightDifference / 2f, transform.position.z);

            _isCrouch = true;
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame && _isCrouch)
        {
            transform.localScale = _standingScale;
            transform.position = new Vector3(transform.position.x, _standingPositionY, transform.position.z);
            _isCrouch = false;
        }

        if (Keyboard.current.shiftKey.IsPressed() && _isGrounded && !_isCrouch)
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
        float facingSign = modelFacesBackward ? -1f : 1f;
        Vector3 moveDir = (transform.forward * _moveInput.y + transform.right * _moveInput.x) * facingSign;
        
        Vector3 targetVelocity = moveDir * moveSpeed;
        Vector3 newVelocity = Vector3.MoveTowards(new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z), targetVelocity, acceleration * Time.fixedDeltaTime);

        _rb.linearVelocity = new Vector3(newVelocity.x, _rb.linearVelocity.y, newVelocity.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}