using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public float fireRange = 100f;
    public Camera playerCam;

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Left mouse button or controller trigger
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, fireRange))
        {
            DroneHealth drone = hit.collider.GetComponent<DroneHealth>();
            if (drone != null)
            {
                drone.TakeDamage(1);
            }
        }
    }
}
