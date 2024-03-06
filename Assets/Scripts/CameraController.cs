using UnityEngine;

public class SimpleCameraControl : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 100.0f;
    public bool enableCollision = true; // Serialized bool to toggle collision detection
    public LayerMask collisionLayerMask; // Define which layers the camera should collide with

    private float xRotation = 0f;
    private float yRotation = 0f;

    // Collider settings
    public float sphereRadius = 0.5f; // Radius for SphereCast
    public float collisionOffset = 0.1f; // Offset to prevent clipping into collision objects

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)){
            enableCollision = !enableCollision;
        }
        RotateCamera();
        Vector3 movement = GetInputMovement();
        if (enableCollision)
        {
            MoveWithCollision(movement);
        }
        else
        {
            transform.position += movement;
        }

        ToggleCursorVisibility();
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Quaternion cameraTurnAngle = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.localRotation = cameraTurnAngle;
    }

    Vector3 GetInputMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        return move * movementSpeed * Time.deltaTime;
    }

    void MoveWithCollision(Vector3 movement)
    {
        int maxCollisions = 3; // Set to the maximum number of collisions you expect to handle
        int collisionCount = 0;

        while (movement.magnitude > 0f && collisionCount < maxCollisions)
        {
            RaycastHit hit;
            Vector3 direction = movement.normalized;
            float distance = movement.magnitude + sphereRadius + collisionOffset;

            // Perform a SphereCast in the direction of movement to check for collisions
            if (Physics.SphereCast(transform.position, sphereRadius, direction, out hit, distance, collisionLayerMask))
            {
                // Calculate movement parallel to the collision surface
                Vector3 slideMovement = Vector3.ProjectOnPlane(movement, hit.normal);

                // Reduce the movement by a tiny factor to prevent sticking to surfaces due to floating point precision errors
                slideMovement *= 0.98f;

                movement = slideMovement;
                collisionCount++;
            }
            else
            {
                transform.position += movement;
                break; // No collision, apply remaining movement and exit loop
            }
        }
    }


    void ToggleCursorVisibility()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
