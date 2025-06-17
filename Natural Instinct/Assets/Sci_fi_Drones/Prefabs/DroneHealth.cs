using UnityEngine;

public class DroneHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    [SerializeField] private GameObject explosionPrefab;

    int hp;

    void Awake() => hp = maxHP;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    void Die()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
