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
    [SerializeField] private float ceilingCheckRadius = 0.2f;
    [SerializeField] private LayerMask ceilingMask;

    [Header("Model Orientation")]
    [SerializeField] private bool modelFacesBackward = false;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _isCrouch = false;

    private Vector3 _standingScale;
    private float _standingPositionY;
    private float _standingGroundCheckLocalY;

    // Deferred crouch/stand request, applied in FixedUpdate for physics consistency
    private bool _crouchRequested = false;
    private bool _standRequested = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _standingScale = transform.localScale;

        if (groundCheck != null)
        {
            _standingGroundCheckLocalY = groundCheck.localPosition.y;
        }
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (!_isCrouch && _isGrounded)
            {
                _crouchRequested = true;
            }
            else if (_isCrouch && HasRoomToStand())
            {
                _standRequested = true;
            }
        }

        if (Keyboard.current.shiftKey.IsPressed() && _isGrounded && !_isCrouch)
        {
            acceleration = sprintAcceleration;
            moveSpeed = sprintSpeed;
        }
        else
        {
            acceleration = 25f;
            moveSpeed = 6f;
        }
    }

    private void FixedUpdate()
    {
        if (_crouchRequested)
        {
            DoCrouch();
            _crouchRequested = false;
        }
        else if (_standRequested)
        {
            DoStand();
            _standRequested = false;
        }

        float facingSign = modelFacesBackward ? -1f : 1f;
        Vector3 moveDir = (transform.forward * _moveInput.y + transform.right * _moveInput.x) * facingSign;

        Vector3 targetVelocity = moveDir * moveSpeed;
        Vector3 newVelocity = Vector3.MoveTowards(new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z), targetVelocity, acceleration * Time.fixedDeltaTime);

        _rb.linearVelocity = new Vector3(newVelocity.x, _rb.linearVelocity.y, newVelocity.z);
    }

    private void DoCrouch()
    {
        _standingPositionY = transform.position.y;
        float heightDifference = _standingScale.y - crouchHeight;

        transform.localScale = new Vector3(_standingScale.x, crouchHeight, _standingScale.z);

        Vector3 newPos = new Vector3(_rb.position.x, _standingPositionY - heightDifference / 2f, _rb.position.z);
        _rb.MovePosition(newPos);

        // Keep groundCheck's world-space offset constant despite the parent's scale change
        if (groundCheck != null)
        {
            groundCheck.localPosition = new Vector3(
                groundCheck.localPosition.x,
                _standingGroundCheckLocalY * (_standingScale.y / crouchHeight),
                groundCheck.localPosition.z);
        }

        // Kill residual vertical velocity so the crouch snap doesn't carry old momentum
        Vector3 vel = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(vel.x, 0f, vel.z);

        _isCrouch = true;
    }

    private void DoStand()
    {
        transform.localScale = _standingScale;

        Vector3 newPos = new Vector3(_rb.position.x, _standingPositionY, _rb.position.z);
        _rb.MovePosition(newPos);

        if (groundCheck != null)
        {
            groundCheck.localPosition = new Vector3(
                groundCheck.localPosition.x,
                _standingGroundCheckLocalY,
                groundCheck.localPosition.z);
        }

        Vector3 vel = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(vel.x, 0f, vel.z);

        _isCrouch = false;
    }

    private bool HasRoomToStand()
    {
        float heightDifference = _standingScale.y - crouchHeight;
        Vector3 origin = transform.position + Vector3.up * (crouchHeight / 2f);
        float castDistance = heightDifference;

        bool blocked = Physics.SphereCast(origin, ceilingCheckRadius, Vector3.up, out _, castDistance, ceilingMask);
        return !blocked;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}