using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float speed = 100f;
    public float maxDistance = 100f;
    public AudioClip hitClip;
    public int damage = 1;
    public GameObject dropPrefab; // Prefab to drop when destroyed

    private Vector3 startPoint;
    private AudioSource audioSource;
    private bool hasHit = false;


    void Start()
    {
        startPoint = transform.position;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Init(AudioClip hitSound)
    {
        hitClip = hitSound;
    }

    void Update()
    {
        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;

        // Self-destruct if too far
        if (Vector3.Distance(startPoint, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayHitSound();

        // TODO: Add damage or effects if needed here
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        DroneHealth health = other.GetComponent<DroneHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            hasHit = true; // 🛡 Prevent multiple hits
            Destroy(gameObject);
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




    void PlayHitSound()
    {
        if (hitClip != null)
        {
            audioSource.PlayOneShot(hitClip);
        }
    }
}
