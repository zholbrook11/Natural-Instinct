using UnityEngine;

public class DroneWander : MonoBehaviour
{
    public float speed = 1.0f;
    public float directionChangeInterval = 3.0f;
    public float fixedHeight = 3.0f;
    public float turnSpeed = 2.0f;
    public float boundaryRadius = 10.0f;
    public float obstacleAvoidanceDistance = 1.0f;

    private Vector3 movementDirection;
    private float timeToChangeDirection;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        ChooseNewDirection();
    }

    void Update()
    {
        Vector3 position = transform.position;

        // Keep drone at fixed height
        position.y = fixedHeight;
        transform.position = position;

        // Obstacle avoidance
        if (Physics.Raycast(transform.position, movementDirection, obstacleAvoidanceDistance))
        {
            ChooseNewDirection();
        }

        // Boundary constraint
        if (Vector3.Distance(startPosition, transform.position) > boundaryRadius)
        {
            movementDirection = (startPosition - transform.position).normalized;
        }

        // Smooth rotation towards movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Change direction after interval
        timeToChangeDirection -= Time.deltaTime;
        if (timeToChangeDirection <= 0)
        {
            ChooseNewDirection();
        }
    }

    void ChooseNewDirection()
    {
        // Pick a new random direction in the X-Z plane
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        movementDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        timeToChangeDirection = directionChangeInterval;
    }
}