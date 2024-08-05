using System.Runtime.CompilerServices;
using UnityEngine;

public class AdvancedMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float slideForce;
    public float maxSlideTime;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

   
    private KeyCode slideKey;

    private bool sliding;
    private PlayerMovement.MovementState movementState;
    private Vector3 inputDirection;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        slideKey = pm.crouchKey;
        startYScale = playerObj.localScale.y;

        movementState = pm.GetMovementState();
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = pm.GetInputDirection();

        // Initiate slide
        if (Input.GetKeyDown(slideKey) && (inputDirection.magnitude != 0))
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

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z); // scales the player down
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // stop the player from floating

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        rb.AddForce(inputDirection * slideForce * Time.deltaTime, ForceMode.Force); // slide in the direction of the input

        slideTimer -= Time.deltaTime; // decrease the slide timer

        if (slideTimer <= 0)
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

    public PlayerMovement.MovementState GetMovementState()
    {
        return movementState;
    }
}
