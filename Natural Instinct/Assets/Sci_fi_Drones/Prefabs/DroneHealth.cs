using UnityEngine;

public class DroneHealth : MonoBehaviour
{
    public GameObject explosionEffectPrefab;

    public void TakeDamage(int amount)
    {
        Die();
    }

    void Die()
{
    if (explosionEffectPrefab)
    {
        GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            ps.Play();
            Destroy(explosion, ps.main.duration);
        }
        else
        {
            Destroy(explosion, 2f); // fallback in case no ParticleSystem is attached
        }
    }

    Destroy(gameObject); // Destroy the drone itself
}

}
