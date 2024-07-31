using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Initialises the necessary variables.
    [Header("Movement")]
    public float walkSpeed;
    public float groundDrag; // base ground friction
    private float moveSpeed; // max speed at a given tinme

    [Header("Jumping")]
    public float jumpForce; // The force applied to the player when they jump.
    public float jumpCooldown; // time between jumps
    public float airMultiplier; // how much faster or slower the player moves in the air
    private bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey;
    public KeyCode crouchKey;

    [Header("Ground Detection")]
    public float playerHeight; // Used for the length of the raycast that detects the ground.
    public LayerMask whatIsGround; // Needed to know what is considered ground.
    private bool grounded; // Flags if the player is on the ground.

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Other")]
    public Transform orientation; // Used to determine the direction of the player's movement.
    public enum MovementState // using an enum will allow me to add other states later on.
    {
        walking,
        air,
        crouching
    }
    private MovementState movementState;

    // wasd input
    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection; // final move direction

    Rigidbody rb; // physics

    void Start()
    {
        // Gets the rigidbody component of the player.
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y; // default y scale
    }

    void Update()
    {
        // Do movement stuff
        PlayerInput();
        MovePlayer();
        SpeedControl();
        StateHandler(); // handles scale and speed of the player
    }

    void PlayerInput() // Handles stuff to do with player input.
    {
        // Gets the input from the player.
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump action
        if (Input.GetKeyDown(jumpKey) && grounded && readyToJump)
        {
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer() // Handles the movement of the player.
    {
        //Calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // adds force to the player in the direction of the movement input
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 100f * Time.deltaTime, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 100f * airMultiplier * Time.deltaTime, ForceMode.Force);
        }
        ApplyDrag(); // Applies the appropriate drag

        // Also I am aware that I'm checking if the player is grounded twice. If performance becomes a problem I will fix it later.
    }

    void ApplyDrag() // Calculates the drag to be applied based on the player's velocity. (totally realistic physics going on here!)
    {
        // Check for ground with a raycast    
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // handle drag
        if (grounded && moveDirection.magnitude == 0 && rb.velocity.magnitude != 0) // If the player is on the ground, not pressing a movement key, and still has velocity.
        {
            rb.drag = groundDrag; // Apply the ground drag.
        }
        else if (grounded && moveDirection.magnitude != 0) // If the player is on the ground and pressing a movement key.
        {
            rb.drag = groundDrag * 0.1f; // Apply reduced drag.
        }
        else
        {
            rb.drag = 0f; // if the player is not on the ground, do no drag
        }
    }

    void SpeedControl() // Ensures that the player does not exceed the maximum speed.
    {
        if (rb.velocity.magnitude > moveSpeed) // If the player's velocity is greater than the move speed.
        {
            rb.velocity = rb.velocity.normalized * moveSpeed; // Set the player's velocity to the move speed.
        }
    }

    void Jump()
    {
        readyToJump = false;

        // reset y velocity to ensure consistent jump height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // Apply the jump force.
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void StateHandler() // scales the player height and movement speed appropriately
    {
        if (grounded && Input.GetKey(crouchKey)) // crouching
        {
            movementState = MovementState.crouching;
            moveSpeed = crouchSpeed;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z); // scales the player down

            if (Input.GetKeyDown(crouchKey)) // only add the downward force upon the key press, as opposed to continually
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // stops player from floating
            }
        }
        else if (grounded) // walking
        {
            movementState = MovementState.walking;
            moveSpeed = walkSpeed;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z); // fixes permanent crouching
        }
        else // air
        {
            movementState = MovementState.air;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z); // fixes permanent crouching
        }
    }

}