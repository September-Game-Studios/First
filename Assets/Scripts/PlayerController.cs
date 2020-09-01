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
    private float turnSmoothVelocity;
    private Vector3 playerVelocity;
    private bool jumped = false;
    
    private float verticalForce = 0.0f;
    private bool groundedPlayer;

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
        this.direction = context.ReadValue<Vector2>();
        if (context.performed)
        {
            Debug.Log("MOVE: " + context.ReadValue<Vector2>());
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            controller.Move(playerVelocity * Time.deltaTime);
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
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            controller.Move(moveDirection * speed * Time.deltaTime);
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
