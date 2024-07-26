using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // Start is called before the first frame update

    // Set up the variables needed
        public float mouseSensitivity = 100.0f;
        public Transform playerBody;
        float xRotation = 0.0f; // The current rotation around the x axis (vertical rotation)

    void Start()
    {
        // lock the cursor in the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust xRotation by the vertical mouse movement
        xRotation += mouseY;

        // Clamp the vertical rotation to avoid flipping
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body along the y-axis by the horizontal mouse movement
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
