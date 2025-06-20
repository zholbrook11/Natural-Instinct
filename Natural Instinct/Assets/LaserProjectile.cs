using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float speed = 100f;
    public float maxDistance = 100f;
    public AudioClip hitClip;

    private Vector3 startPoint;
    private AudioSource audioSource;

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
        PlayHitSound();
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
