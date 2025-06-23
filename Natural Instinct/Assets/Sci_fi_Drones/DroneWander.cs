using UnityEngine;

public class DroneWander : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1.0f;
    public float directionChangeInterval = 3.0f;
    public float fixedHeight = 3.0f;
    public float turnSpeed = 2.0f;
    public float boundaryRadius = 10.0f;
    public float obstacleAvoidanceDistance = 1.0f;

    [Header("Hit & Destroy Settings")]
    public string projectileTag = "Projectile"; // Tag of the projectile
    public GameObject dropPrefab; // Prefab to drop when destroyed

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
        // Maintain fixed height
        Vector3 position = transform.position;
        position.y = fixedHeight;
        transform.position = position;

        // Obstacle avoidance
        if (Physics.Raycast(transform.position, movementDirection, obstacleAvoidanceDistance))
        {
            ChooseNewDirection();
        }

        // Stay within boundary
        if (Vector3.Distance(startPosition, transform.position) > boundaryRadius)
        {
            movementDirection = (startPosition - transform.position).normalized;
        }

        // Smoothly rotate toward movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Time to change direction
        timeToChangeDirection -= Time.deltaTime;
        if (timeToChangeDirection <= 0)
        {
            ChooseNewDirection();
        }
    }

    void ChooseNewDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        movementDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        timeToChangeDirection = directionChangeInterval;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(projectileTag))
        {
            HandleHit();
        }
    }

    private void HandleHit()
    {
        // Drop prefab if assigned
        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

        // Optional: play FX, sound, or animation here

        Destroy(gameObject);
    }
}
