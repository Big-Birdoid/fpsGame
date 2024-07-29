using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Initialises the necessary variables.
    public float moveSpeed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        InputDirection();
        MovePlayer();
    }

    void InputDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    void MovePlayer()
    {
        // adds force to the player in the direction of the movement input
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * Time.deltaTime, ForceMode.Force);
    }

}