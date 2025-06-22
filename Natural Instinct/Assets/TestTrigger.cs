using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    [SerializeField] private DroneWaveSpawner spawner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawner.TestStartWave();
        }
    }
}

