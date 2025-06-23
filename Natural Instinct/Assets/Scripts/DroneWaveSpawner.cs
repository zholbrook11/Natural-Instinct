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
        spawnPoints = System.Array.FindAll(spawnPoints, t => t != transform);
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
        Debug.Log("Drone destroyed. Drones remaining: " + dronesAlive); // 🔍

        if (dronesAlive <= 0)
        {
            Debug.Log("All drones down. Spawning tesseract."); // 🔍
            Instantiate(tesseractPrefab, tesseractSpawnPoint.position, tesseractSpawnPoint.rotation);
        }
    }

}
