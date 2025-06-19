using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection;
    private float rotationX = 0;

    void Update()
    {
        // Get movement input
        float moveForward = Input.GetAxis("Vertical");
        float moveSide = Input.GetAxis("Horizontal");

        moveDirection = transform.forward * moveForward + transform.right * moveSide;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.parent.Rotate(Vector3.up * mouseX); // Rotate the parent object for yaw
    }
}