using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of camera movement
    public float zoomSpeed = 10f; // Speed of camera zooming
    public float minSize = 1f; // Minimum size of camera orthographic view
    public float maxSize = 30f; // Maximum size of camera orthographic view

    private float currentSpeed; // Current speed of camera movement
    private bool cameraControlEnabled = true; // Flag to enable/disable camera control

    void Update()
    {
        // Toggle camera control when Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraControlEnabled = !cameraControlEnabled;
        }

        // If camera control is enabled, move and zoom the camera
        if (cameraControlEnabled)
        {
            MoveCamera(); // Move the camera based on input
            ZoomCamera(); // Zoom the camera based on input
        }
    }

    void MoveCamera()
    {
        float verticalInput = Input.GetAxis("Vertical"); // Get vertical input (W and S keys)
        float horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input (A and D keys)

        // Calculate the movement direction and move the camera accordingly
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void ZoomCamera()
    {
        float zoomAmount = zoomSpeed; // Default zoom speed

        // Zoom in when Q key is pressed
        if (Input.GetKey(KeyCode.Q))
        {
            ChangeCameraSize(-zoomAmount * Time.deltaTime);
        }

        // Zoom out when E key is pressed
        if (Input.GetKey(KeyCode.E))
        {
            ChangeCameraSize(zoomAmount * Time.deltaTime);
        }
    }

    void ChangeCameraSize(float increment)
    {
        // Change the orthographic size of the camera to zoom in or out
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + increment, minSize, maxSize);
    }
}
