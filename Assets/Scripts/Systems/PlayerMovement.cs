using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    
    public float jumpHeight = 4f;
    public float fallMultiplier = 1.0f; // Как быстро игрок возвращается на землю
    public float ascendMultiplier = 1.0f; // Как быстро игрок поднимается до пика прыжка
    [SerializeField] private LayerMask groundLayer; // Проверят какие слои являются землёй
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
    
    // для прыжка
    private bool isGrounded = true;

    // Для камеры
    private Transform cameraTransform;
    private float pitch; // текущий угол наклона
    private void Awake()
    {
        controls = new PlayerControls();
        playerRigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        cameraTransform = Camera.main.transform;
        originalCameraLocalPos = cameraTransform.localPosition;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerRigidbody.freezeRotation = true;
        Cursor.visible = false;
        raycastDistance = (standHeight / 2) + 0.2f;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    
    private void Update()
    {
        Move();
        ApplyGravity();
        Rotate();
    }
    
    void FixedUpdate()
    {
        ApplyGravity();
    }

    private void Move()
    {
        // ————— ДВИЖЕНИЕ —————
        Vector2 moveDirection = controls.Player.Move.ReadValue<Vector2>();
        bool isRun = controls.Player.Run.IsPressed();
        bool isCrouching = controls.Player.Crouch.IsPressed();
        float speed = isRun ? runSpeed : walkSpeed;
        capsule.height = isCrouching ? crouchHeight : standHeight;

        Vector3 dir = (
            transform.right * moveDirection.x + transform.forward * moveDirection.y
            ).normalized;
        Vector3 targetVelocity = dir * speed;

        // Сохраняем текущую вертикальную скорость (для гравитации и прыжка)
        targetVelocity.y = playerRigidbody.linearVelocity.y;

        // Применяем скорость
        playerRigidbody.linearVelocity = targetVelocity;

        if (isGrounded && moveDirection == Vector2.zero)
        {
            playerRigidbody.linearVelocity = new Vector3();
        }

        // ————— ПРЫЖОК —————
        
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

        // ————— СЕСТЬ —————
        
        // меняем центр коллайдера, чтобы он оставался на земле
        capsule.center = new Vector3(0, capsule.height / 2f, 0);

        // Плавно опускаем/поднимаем камеру
        Vector3 targetCamPos = 
            originalCameraLocalPos 
            + 
            (isCrouching ? new Vector3(0, crouchCameraOffset, 0) : Vector3.zero);
        
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, targetCamPos, Time.deltaTime * 10f
            );
    }

    private void Jump()
    {
            isGrounded = false;
            groundCheckTimer = groundCheckDelay;
            playerRigidbody.linearVelocity = new Vector3(
                playerRigidbody.linearVelocity.x, jumpHeight, playerRigidbody.linearVelocity.z
            );
    }

    private void Rotate()
    {
        // Читаем дельту мыши 
        Vector2 delta = controls.Player.Look.ReadValue<Vector2>();
        
        // Применяем чувствительность
        float yaw   = delta.x * lookSensitivity;
        float pitchDelta = delta.y * lookSensitivity;
        
        //Поворачиваем всё тело персонажа по горизонтали
        transform.Rotate(Vector3.up, yaw);
        
        //  Наклоняем камеру по вертикали
        pitch -= pitchDelta;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
    
    private void ApplyGravity()
    {
        // ————— ГРАВИТАЦИЯ —————
        if (playerRigidbody.linearVelocity.y < 0) 
        {
            // Falling: Apply fall multiplier to make descent faster
            playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        } // Rising
        else if (playerRigidbody.linearVelocity.y > 0)
        {
            // Rising: Change multiplier to make player reach peak of jump faster
            playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * ascendMultiplier  * Time.fixedDeltaTime;
        }
    }
}