using UnityEngine;
using UnityEngine.Events;

public class DroneHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    [SerializeField] private GameObject explosionPrefab;

    public UnityAction<DroneHealth> OnDroneDestroyed; // 🔸 Added this line

    int hp;

    void Awake() => hp = maxHP;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    void Die()
    {
        OnDroneDestroyed?.Invoke(this); // 🔸 Notify listeners BEFORE destruction

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
