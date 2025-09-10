using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.3f;

    [Header("Jump")]
    public float jumpHeight = 1.0f;
    private float jumpForce;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Sliding")]
    public float slideThreshold = 0.6f;
    public float slideForce = 5f;
    public float maxSlideSpeed = 10f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpRequested;

    private Vector3 currentMovementVelocity;
    private float currentVelocityX;
    private float currentVelocityZ;

    // Input System
    private PlayerControls controls;
    private Vector2 moveInput;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Gameplay.Jump.performed += ctx => { if (isGrounded) jumpRequested = true; };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        jumpForce = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpHeight);

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 rawInput = new Vector3(moveInput.x, 0, moveInput.y);

        // Rotate input to match isometric view (-45 degrees)
        float angle = -45f * Mathf.Deg2Rad;
        Vector3 moveDirection = new Vector3(
            rawInput.x * Mathf.Cos(angle) - rawInput.z * Mathf.Sin(angle),
            0,
            rawInput.x * Mathf.Sin(angle) + rawInput.z * Mathf.Cos(angle)
        );

        float inputMagnitude = moveDirection.magnitude;

        Vector3 targetMovement = moveDirection.normalized * moveSpeed * Mathf.Clamp01(inputMagnitude);

        if (inputMagnitude > 0.01f)
        {
            currentMovementVelocity.x = Mathf.SmoothDamp(currentMovementVelocity.x, targetMovement.x, ref currentVelocityX, accelerationTime);
            currentMovementVelocity.z = Mathf.SmoothDamp(currentMovementVelocity.z, targetMovement.z, ref currentVelocityZ, accelerationTime);
        }
        else
        {
            currentMovementVelocity.x = Mathf.SmoothDamp(currentMovementVelocity.x, 0, ref currentVelocityX, decelerationTime);
            currentMovementVelocity.z = Mathf.SmoothDamp(currentMovementVelocity.z, 0, ref currentVelocityZ, decelerationTime);
        }

        // Apply movement
        rb.linearVelocity = new Vector3(currentMovementVelocity.x, rb.linearVelocity.y, currentMovementVelocity.z);

        // Rotate player
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        UpdateAnimations(rb.linearVelocity);
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            CheckForSliding();

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpRequested = false;
        }
    }

    private void CheckForSliding()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            float slopeCosine = Mathf.Cos(slopeAngle * Mathf.Deg2Rad);

            if (slopeCosine < slideThreshold)
            {
                Vector3 slideDir = new Vector3(hit.normal.x, 0, hit.normal.z).normalized;
                Vector3 slideVelocity = slideDir * slideForce;

                float currentSlideSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
                if (currentSlideSpeed < maxSlideSpeed)
                    rb.AddForce(slideVelocity, ForceMode.Acceleration);
            }
        }
    }

    private void UpdateAnimations(Vector3 velocity)
    {
        if (animator == null) return;

        Vector3 flatVelocity = new Vector3(velocity.x, 0, velocity.z);
        float speed = flatVelocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsWalking", speed > 0.1f);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
