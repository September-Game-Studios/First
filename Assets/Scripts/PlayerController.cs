using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputMaster.IPlayerActions
{
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    private InputMaster controls;
    private CharacterController controller;
    private Vector2 direction;
    private Vector3 playerVelocity;
    private float turnSmoothVelocity;
    private bool groundedPlayer;

    // Dashing
    public float dashSpeed = 12f;
    public float dashDuration = 0.5f;
    private float dashDurationTimer = 0.0f;
    public float dashCooldown = 2.0f;
    private float dashCooldownTimer = 0.0f;
    private bool isDashing = false;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        controls = new InputMaster();
        controls.Player.SetCallbacks(this);
    }

    public void OnEnable()
    {
        controls.Player.Enable();
    }

    public void OnDisable()
    {
        controls.Player.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!isDashing)
        {
            this.direction = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && groundedPlayer && !isDashing)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            controller.Move(playerVelocity * Time.deltaTime);
        }        
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && dashCooldownTimer == 0.0f && direction.magnitude >= 0.1f)
        {
            Debug.Log("DASH");
            isDashing = true;
            dashDurationTimer = 0.0f;
        }
    }

    void Update()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (direction.magnitude >= 0.1f)
        {
            float currentSpeed = isDashing ? dashSpeed : speed;
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        if (dashCooldownTimer > 0.0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0.0f)
            {
                dashCooldownTimer = 0.0f;
            }
        }

        if (isDashing)
        {
            dashDurationTimer += Time.deltaTime;
            if (dashDurationTimer >= dashDuration)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
        }
    }
    private void FixedUpdate()
    {
        ApplyGravity();
        groundedPlayer = controller.isGrounded;
    }

    private void ApplyGravity()
    {
        playerVelocity.y += gravityValue * Time.fixedDeltaTime;
        controller.Move(playerVelocity * Time.fixedDeltaTime);
    }
}
