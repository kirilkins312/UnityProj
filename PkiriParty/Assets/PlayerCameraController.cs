using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity of mouse movement
    public Transform playerBody; // Reference to the player's body (usually the root transform)
    
    float xRotation = 0f; // Vertical rotation of the camera
    float currentYRotation = 0f; // Current horizontal rotation of the camera

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Optional, to hide the cursor
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Restrict looking too far up or down

        // Apply vertical rotation to the camera (local rotation)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (yaw) applied to the player body (global rotation)
        currentYRotation += mouseX;
        playerBody.rotation = Quaternion.Euler(0f, currentYRotation, 0f); // Rotate player to match camera
    }
}
