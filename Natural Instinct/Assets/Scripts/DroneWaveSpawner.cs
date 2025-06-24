using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWaveSpawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject tesseractPrefab;
    [SerializeField] private Transform tesseractSpawnPoint;

    [Header("Spawn Settings")]
    [SerializeField] private int initialDrones = 10;
    [SerializeField] private int waveDrones = 5;
    [SerializeField] private float waveInterval = 5f;

    private Transform[] spawnPoints;
    private bool tesseractGrabbed = false;

    private Coroutine waveSpawnerCoroutine;

    void Awake()
    {
        // Get all spawn point children, skipping index 0 (self)
        List<Transform> points = new List<Transform>(GetComponentsInChildren<Transform>());
        points.RemoveAt(0); // remove the parent
        spawnPoints = points.ToArray();
    }

    public void TestStartWave()
    {
        SpawnDrones(initialDrones);
    }

    private void SpawnDrones(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
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
        drone.OnDroneDestroyed -= HandleDroneDestroyed;

        // 25% chance to drop a tesseract
        if (!tesseractGrabbed && Random.value <= 0.25f)
        {
            Instantiate(tesseractPrefab, tesseractSpawnPoint.position, tesseractSpawnPoint.rotation);
        }
    }

    public void OnTesseractGrabbed()
    {
        if (tesseractGrabbed) return;

        tesseractGrabbed = true;

        // Start infinite drone wave spawner
        waveSpawnerCoroutine = StartCoroutine(SpawnDroneWaves());
    }

    private IEnumerator SpawnDroneWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);
            SpawnDrones(waveDrones);
        }
    }
}

