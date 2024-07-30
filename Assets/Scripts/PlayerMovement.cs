using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    // Initialises the necessary variables.
    public float moveSpeed; // public so i can adjust it in the inspector.
    public float groundDrag; // The drag applied to the player when they are on the ground.

    [Header("Ground Detection")]
    public float playerHeight; // Used for the length of the raycast that detects the ground.
    public LayerMask whatIsGround; // Needed to know what is considered ground.
    bool grounded; // Flags if the player is on the ground.

    [Header("Transforms")]
    public Transform orientation; // Used to determine the direction of the player's movement.

    // wasd input
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection; // final move direction

    Rigidbody rb; // physics

    void Start()
    {
        // Gets the rigidbody component of the player.
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Do movement stuff
        InputDirection();
        MovePlayer();
    }

    void InputDirection() // Determines the direction of the player's movement.
    {
        // Gets the input from the player.
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    void MovePlayer()
    {
        // adds force to the player in the direction of the movement input
        rb.AddForce(moveDirection.normalized * moveSpeed * 100f * Time.deltaTime, ForceMode.Force); // Changed the multiplier to 100f so that the player actually moves.
        ApplyDrag();
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
            rb.drag = groundDrag * 0.1f; // Apply the ground drag.
        }
        else
        {
            rb.drag = 0f; // if the player is not on the ground, do no drag
        }
    }

}