using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    // Initialises the necessary variables.
    public float moveSpeed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        // Gets the rigidbody component of the player.
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
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
    }

}