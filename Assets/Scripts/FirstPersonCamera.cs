using UnityEngine;

// Forgot to mention - here is the fixed code from the last commits
public class FirstPersonCamera : MonoBehaviour
{

    // Set up the variables needed
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    void Start()
    {
        // lock the cursor in the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        // Adjust rotations
        xRotation -= mouseY;
        yRotation += mouseX;

        // Clamp rotation to avoid flipping
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
