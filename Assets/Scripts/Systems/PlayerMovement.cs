using UnityEngine;

/// <summary>
/// Handles player movement, jumping, crouching, and camera look controls.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    
    public float jumpHeight = 4f;
    public float fallMultiplier = 1.0f; // How fast player returns to the ground
    public float ascendMultiplier = 1.0f; // How fast player reaches pinnacle of the jump
    [SerializeField] private LayerMask groundLayer; // Defines which layers are the ground
    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float raycastDistance;
    
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchCameraOffset = -0.5f;
    
    public float lookSensitivity = 0.5f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private PlayerControls controls;
    private Vector3 velocity;
    private Rigidbody playerRigidbody;
    private CapsuleCollider capsule;
    private Vector3 originalCameraLocalPos;
    
    // True if we are on the ground
    private bool isGrounded = true;

    // Camera members
    private Transform cameraTransform;
    private float pitch; // текущий угол наклона
    
    /// <summary>
    /// Initializes components and input controls.
    /// </summary>
    private void Awake()
    {
        controls = new PlayerControls();
        playerRigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        cameraTransform = Camera.main.transform;
        originalCameraLocalPos = cameraTransform.localPosition;
    }

    /// <summary>
    /// Locks cursor and configures physics settings.
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerRigidbody.freezeRotation = true;
        Cursor.visible = false;
        raycastDistance = (standHeight / 2) + 0.2f;
    }

    /// <summary>
    /// Enables input when component is active.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables input when component is inactive.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }
    
    /// <summary>
    /// Processes movement, gravity, and rotation each frame.
    /// </summary>
    private void Update()
    {
        Move();
        Rotate();
    }
    
    /// <summary>
    /// Applies gravity in the physics update step.
    /// </summary>
    void FixedUpdate()
    {
        ApplyGravity();
    }

    /// <summary>
    /// Reads input and moves the player, handles jumping and crouching.
    /// </summary>
    private void Move()
    {
        // ————— MOVEMENT —————
        Vector2 moveDirection = controls.Player.Move.ReadValue<Vector2>();
        bool isRun = controls.Player.Run.IsPressed();
        bool isCrouching = controls.Player.Crouch.IsPressed();
        float speed = isRun ? runSpeed : walkSpeed;
        capsule.height = isCrouching ? crouchHeight : standHeight;

        Vector3 dir = (
            transform.right * moveDirection.x + transform.forward * moveDirection.y
            ).normalized;
        Vector3 targetVelocity = dir * speed;

        // Save current vertical velocity (for gravity and jump)
        targetVelocity.y = playerRigidbody.linearVelocity.y;

        // Apply velocity
        playerRigidbody.linearVelocity = targetVelocity;

        if (isGrounded && moveDirection == Vector2.zero)
        {
            playerRigidbody.linearVelocity = new Vector3();
        }

        // ————— JUMP —————
        
        if (controls.Player.Jump.triggered && isGrounded)
        {
            Jump();
        }
        
        if (!isGrounded && groundCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
        }
        else
        {
            groundCheckTimer -= Time.deltaTime;
        }

        // ————— CROUCH —————
        
        // Change collision centre, so it stays on the ground
        capsule.center = new Vector3(0, capsule.height / 2f, 0);

        // Smoothly lower/raise the camera
        Vector3 targetCamPos = 
            originalCameraLocalPos 
            + 
            (isCrouching ? new Vector3(0, crouchCameraOffset, 0) : Vector3.zero);
        
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, targetCamPos, Time.deltaTime * 10f
            );
    }

    /// <summary>
    /// Initiates a jump by setting upward velocity.
    /// </summary>
    private void Jump()
    {
            isGrounded = false;
            groundCheckTimer = groundCheckDelay;
            playerRigidbody.linearVelocity = new Vector3(
                playerRigidbody.linearVelocity.x, jumpHeight, playerRigidbody.linearVelocity.z
            );
    }

    /// <summary>
    /// Handles player yaw and camera pitch based on mouse input.
    /// </summary>
    private void Rotate()
    {
        // Read mouse delta
        Vector2 delta = controls.Player.Look.ReadValue<Vector2>();
        
        // Apply sensitivity
        float yaw   = delta.x * lookSensitivity;
        float pitchDelta = delta.y * lookSensitivity;
        
        // Rotate the entire character's body horizontally
        transform.Rotate(Vector3.up, yaw);
        
        //  Tilt the camera vertically
        pitch -= pitchDelta;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
    
    /// <summary>
    /// Applies enhanced gravity when ascending or descending.
    /// </summary>
    private void ApplyGravity()
    {
        // ————— GRAVITY —————
        if (playerRigidbody.linearVelocity.y < 0) 
        {
            // Falling: Apply fall multiplier to make descent faster
            playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
        else if (playerRigidbody.linearVelocity.y > 0)
        {
            // Rising: Change multiplier to make player reach peak of jump faster
            playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * ascendMultiplier  * Time.fixedDeltaTime;
        }
    }
}