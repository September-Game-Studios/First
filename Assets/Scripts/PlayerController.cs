using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    private CharacterController controller;
    private Vector2 direction;
    private Vector3 playerVelocity;
    private float turnSmoothVelocity;
    private bool groundedPlayer;

	private ReachController reach;

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

    [System.Serializable]
    public class Grab
    {
		static readonly int Layer = 9;

        public Transform hands;
		[HideInInspector]
		public ReachController reach;

        private GameObject heldItem = null;

        public void Hold(GameObject item = null)
        {
            item = (item == null) ? reach.GetClosest(Layer) : item;

            heldItem = item;
            heldItem.GetComponent<Rigidbody>().isKinematic = true;
            heldItem.transform.SetParent(hands);
        }

        public void Drop()
        {
            heldItem.GetComponent<Rigidbody>().isKinematic = false;
            heldItem.transform.SetParent(null);
            heldItem = null;
        }

		public bool CanGrab
		{
			get => reach.CanReach(Layer);
		}

        public bool isHolding { get => heldItem != null; }
    }

    public Grab grab;

	// Drag attributes
	GameObject cargo = null;

	private bool holdingCargo
	{
		get => cargo != null;
	}
    
    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
		reach = gameObject.GetComponentInChildren<ReachController>();
		grab.reach = reach;
    }

    public void OnMovement(InputValue value)
    {
        if (!dash.active)
        {
            this.direction = value.Get<Vector2>();
        }
    }

    public void OnJump()
    {
        if ( groundedPlayer && !dash.active)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            controller.Move(playerVelocity * Time.deltaTime);
        }        
    }

    public void OnDash()
    {
        if (!dash.active && dash.cooldownTimer == 0.0f && direction.magnitude >= 0.1f)
        {
            Debug.Log("DASH");
            dash.active = true;
            dash.durationTimer = 0.0f;
        }
    }

    public void OnGrab()
    {
		/*Debug.Log(grab.CanGrab);
        if (grab.isHolding)
        {
            grab.Drop();
        }
        else if (grab.CanGrab)
        {
            grab.Hold();
        }*/
		// DRAG
		
		int layer = LayerMask.NameToLayer("Cargo");
		if (holdingCargo)
		{
			// let go
		}
		else
		{
			if (reach.CanReach(layer))
			{Debug.Log("DRAG");
				// Cargo in reach
				Hold(layer);
			}
		}
    }

	private void Hold(int layer)
	{
		cargo = reach.GetClosest(layer);
		cargo.GetComponent<Rigidbody>().isKinematic = true;
		cargo.transform.SetParent(grab.hands);
	}

	private void LetGo()
	{

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
