using UnityEngine;

public class DroneWander : MonoBehaviour
{
    public float speed = 1.0f;
    public float directionChangeInterval = 3.0f;
    public float fixedHeight = 3.0f;

    private Vector3 movementDirection;
    private float timeToChangeDirection;

    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        // Always stay at the fixed height
        Vector3 position = transform.position;
        position.y = fixedHeight;
        transform.position = position;

        // Move in current direction
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        // Countdown to change direction
        timeToChangeDirection -= Time.deltaTime;
        if (timeToChangeDirection <= 0)
        {
            ChooseNewDirection();
        }
    }

    void ChooseNewDirection()
    {
        // Random direction on X-Z plane
        float angle = Random.Range(0f, 360f);
        movementDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        timeToChangeDirection = directionChangeInterval;
    }
}
