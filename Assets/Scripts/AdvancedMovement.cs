using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class AdvancedMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation; // reference to the orientation object
    public Transform playerObj; // reference to the player object
    private Rigidbody rb; // reference to the rigidbody
    private PlayerMovement pm; // reference to the PlayerMovement script (used composition instead of inheritance) 

    [Header("Sliding")]
    public float slideForce; // the force applied to the player when sliding
    public float maxSlideTime; // the maximum time the player can slide
    private float slideTimer; // counts down from maxSlideTime to 0

    private KeyCode slideKey; // the key used to slide (same as crouch key)

    private bool sliding; // is the player sliding?
    private PlayerMovement.MovementState movementState; // the current movement state of the player
    private Vector3 moveDirection; // the movement direction of the player

    [Header("Wall Running")]
    public LayerMask whatIsWall; // what is considered a wall
    private float wallRunForce; // the force applied to the player when wall running
    public float maxWallRunTime; // the maximum time the player can wall run
    private float wallRunTimer; // counts down from maxWallRunTime to 0

    [Header("Wall Detection")]
    public float wallCheckDistance; // how much distance for raycast checks
    private RaycastHit leftWallHit; // checks for left wall
    private RaycastHit rightWallHit; // checks for right wall
    private bool leftWall; // wall on left?
    private bool rightWall; // wall on right?
    
    void Start()
    {
        // Assigns references to the variables
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        slideKey = pm.crouchKey;

        movementState = pm.GetMovementState();

        wallRunForce = pm.GetMoveSpeed() * 100f; // same force as that of walking in PlayerMovement.cs
    }

    void Update()
    {
        moveDirection = pm.GetMoveDirection(); // get the input direction from PlayerMovement.cs
        WallCheck(); // check for walls

        // Initiate slide
        if (Input.GetKeyDown(slideKey) && moveDirection.magnitude != 0 && pm.GetGrounded())
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && sliding)
        {
            StopSlide();
        }

        if (sliding)
        {
            SlidingMovement();
        }

        // Wall running
        if ((leftWall || rightWall) && !pm.GetGrounded() && pm.GetVerticalInput() != 0)
        {
            if (movementState != PlayerMovement.MovementState.wallRunning)
            {
                StartWalllRun();
            }
            else
            {
                WallRunMovement();
            }
        }
        else
        {
            if (movementState == PlayerMovement.MovementState.wallRunning)
            {
                StopWallRun();
            }
        }
    }

    // Sliding
    private void StartSlide()
    {
        sliding = true; // the player is now sliding
        movementState = PlayerMovement.MovementState.sliding; // makes the speed limiter work as intended
        slideTimer = maxSlideTime; // reset the slide timer
        rb.AddForce(moveDirection * slideForce * 10f * Time.deltaTime, ForceMode.Impulse); // sliding is way cooler now!
    }

    private void SlidingMovement()
    {   
        if (!pm.GetOnSlope() && pm.GetGrounded()) // i want infinite, crazy fast slope sliding
        {
            slideTimer -= Time.deltaTime; // decrease the slide timer
        }
        else if (pm.GetOnSlope() && rb.velocity.y < 0) // going down a slope
        {
            rb.AddForce(moveDirection * slideForce * 2f * Time.deltaTime, ForceMode.Force); // apply the slide force for crazy slope stuff
        }

        if (slideTimer <= 0 && pm.GetGrounded()) // if the player is grounded and the tyimer has run out.
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        sliding = false; // the player is no longer sliding
        movementState = PlayerMovement.MovementState.walking; // this helps with the speed limiter in PlayerMovement.cs
    }
    // Sliding over

    // Wall Running
    private void WallCheck()
    {
        leftWall = Physics.Raycast(playerObj.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall); // check for left wall
        rightWall = Physics.Raycast(playerObj.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall); // check for right wall
    }
    // using pm.GetGrounded() for ground check

    private void StartWalllRun()
    {
        movementState = PlayerMovement.MovementState.wallRunning;
        rb.useGravity = false; // no gravity when wall running
        wallRunTimer = maxWallRunTime; // reset the wall run timer
    }

    private void WallRunMovement()
    {   
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // no vertical velocity

        Vector3 wallNormal = leftWall ? leftWallHit.normal : rightWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        // Forward force for wall running
        rb.AddForce(wallForward * wallRunForce * Time.deltaTime, ForceMode.Force);

        wallRunTimer -= Time.deltaTime; // decrease the wall run timer

        if (wallRunTimer <=0)
        {
            rb.useGravity = true; // gravity is back baby!!! Newton would be proud
        }
    }

    private void StopWallRun()
    {
        movementState = PlayerMovement.MovementState.walking;
        rb.useGravity = true;
    }

    // Getters
    public PlayerMovement.MovementState GetMovementState() // used in PlayerMovement.cs
    {
        return movementState;
    }

    public Vector3 GetWallNormal()
    {
        return leftWall ? leftWallHit.normal : rightWallHit.normal;
    }
}
