using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Drone Settings")]
    public GameObject dronePrefab;

    [Header("Spawn Area Settings")]
    public Vector2 areaSize = new Vector2(10f, 10f); // Width (X), Depth (Z)
    public float spawnHeight = 5f;

    [Header("Spawn Control")]
    public bool enableRandomSpawning = true;
    public float spawnInterval = 3f;

    private void Start()
    {
        if (enableRandomSpawning)
        {
            InvokeRepeating(nameof(SpawnRandomDrone), 1f, spawnInterval);
        }
    }

    void SpawnRandomDrone()
    {
        Vector3 randomPosition = GetRandomSpawnPosition();
        SpawnDrone(randomPosition);
    }

    public void SpawnAtPosition(Vector3 position)
    {
        position.y = spawnHeight; // Force to specified height
        SpawnDrone(position);
    }

    void SpawnDrone(Vector3 position)
    {
        if (dronePrefab != null)
        {
            Instantiate(dronePrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Drone prefab not assigned!");
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        return new Vector3(randomX, spawnHeight, randomZ) + transform.position;
    }

    // Optional: Draw the spawn area in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(0, spawnHeight, 0) + transform.position;
        Vector3 size = new Vector3(areaSize.x, 0.1f, areaSize.y);
        Gizmos.DrawWireCube(center, size);
    }
}
