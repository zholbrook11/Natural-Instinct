using UnityEngine;
using UnityEngine.Events;

public class DroneHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 3;
    private int currentHP;
    private bool isDead = false;

    [Header("Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Tag Settings")]
    [SerializeField] private string projectileTag = "Projectile";

    public UnityAction<DroneHealth> OnDroneDestroyed;

    void Awake()
    {
        currentHP = maxHP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag(projectileTag))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;

        // Play hit sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDroneDestroyed?.Invoke(this);

        // Play explosion sound
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Spawn drop
        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the drone after explosion sound has time to play
        Destroy(gameObject, 0.1f);
    }
}
