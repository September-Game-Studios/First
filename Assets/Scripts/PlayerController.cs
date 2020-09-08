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

    [System.Serializable]
    public class Dash
    {
        public float speed = 12f;
        public float duration = 0.5f;
        public float cooldown = 2.0f;
        [HideInInspector]
        public float durationTimer = 0.0f;
        [HideInInspector]
        public float cooldownTimer = 0.0f;
        [HideInInspector]
        public bool active = false;
    }
    public Dash dash;
    
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
        if (!dash.active)
        {
            this.direction = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && groundedPlayer && !dash.active)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            controller.Move(playerVelocity * Time.deltaTime);
        }        
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !dash.active && dash.cooldownTimer == 0.0f && direction.magnitude >= 0.1f)
        {
            Debug.Log("DASH");
            dash.active = true;
            dash.durationTimer = 0.0f;
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
            float currentSpeed = dash.active ? dash.speed : speed;
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        if (dash.cooldownTimer > 0.0f)
        {
            dash.cooldownTimer -= Time.deltaTime;
            if (dash.cooldownTimer <= 0.0f)
            {
                dash.cooldownTimer = 0.0f;
            }
        }

        if (dash.active)
        {
            dash.durationTimer += Time.deltaTime;
            if (dash.durationTimer >= dash.duration)
            {
                dash.active = false;
                dash.cooldownTimer = dash.cooldown;
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
