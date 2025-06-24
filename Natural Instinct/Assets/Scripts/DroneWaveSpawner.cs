using System.Collections.Generic;
using UnityEngine;

public class DroneWaveSpawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject tesseractPrefab;
    [SerializeField] private Transform tesseractSpawnPoint;

    private Transform[] spawnPoints;
    private int dronesAlive;
    private int dronesPerWave = 5;

    void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    public void TestStartWave()
    {
        if (dronesAlive > 0) return;

        dronesAlive = dronesPerWave;

        for (int i = 1; i <= dronesPerWave; i++)  // skip index 0 (self)
        {
            Transform p = spawnPoints[i % spawnPoints.Length];
            GameObject d = Instantiate(dronePrefab, p.position, p.rotation);

            DroneHealth droneHealth = d.GetComponent<DroneHealth>();
            if (droneHealth != null)
            {
                droneHealth.OnDroneDestroyed += HandleDroneDestroyed;
            }
        }
    }

    private void HandleDroneDestroyed(DroneHealth drone)
    {
        dronesAlive--;
        if (dronesAlive <= 0)
        {
            Instantiate(tesseractPrefab, tesseractSpawnPoint.position, tesseractSpawnPoint.rotation);
        }
    }
}
