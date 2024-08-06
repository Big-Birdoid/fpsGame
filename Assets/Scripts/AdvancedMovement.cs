using UnityEngine;

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

    private float slideYScale; // the scale of the player when sliding (same as crouching)
    private float startYScale; // the scale of the player when not sliding

   
    private KeyCode slideKey; // the key used to slide (same as crouch key)

    private bool sliding; // is the player sliding?
    private PlayerMovement.MovementState movementState; // the current movement state of the player
    private Vector3 inputDirection; // the input direction of the player

    
    void Start()
    {
        // Assigns references to the variables
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        slideKey = pm.crouchKey;
        startYScale = playerObj.localScale.y;

        slideYScale = pm.crouchYScale;

        movementState = pm.GetMovementState();
    }

    void Update()
    {
        inputDirection = pm.GetInputDirection(); // get the input direction from PlayerMovement.cs

        // Initiate slide
        if (Input.GetKeyDown(slideKey) && inputDirection.magnitude != 0 && pm.GetGrounded())
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
    }

    private void StartSlide()
    {
        sliding = true;
        movementState = PlayerMovement.MovementState.sliding; // makes the speed limiter work as intended

        if (pm.GetMovementState() != PlayerMovement.MovementState.crouching) // if the player is not already scaled down
        {
            playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z); // scales the player down
        }
        
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // stop the player from floating

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        rb.AddForce(inputDirection * slideForce * Time.deltaTime, ForceMode.Force); // slide in the direction of the input

        if (!pm.GetOnSlope()) // i want infinite slope sliding
        {
            slideTimer -= Time.deltaTime; // decrease the slide timer
        }

        if (slideTimer == 0 && pm.GetGrounded()) // if the player is grounded and the tyimer has run out.
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        sliding = false;
        movementState = PlayerMovement.MovementState.walking;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z); // scales the player back up
    }

    // Getters
    public PlayerMovement.MovementState GetMovementState() // used in PlayerMovement.cs
    {
        return movementState;
    }
}
