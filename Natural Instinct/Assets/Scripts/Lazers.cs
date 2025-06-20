using UnityEngine;
using UnityEngine.InputSystem;

public class VRGunHand : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference fireAction; // Assign FireLeft or FireRight in Inspector

    [Header("Laser Settings")]
    public GameObject projectilePrefab;
    public Transform muzzleTransform;
    public float projectileSpeed = 80f;

    void OnEnable()
    {
        if (fireAction != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed += FireLaser;
        }
    }

    void OnDisable()
    {
        if (fireAction != null)
        {
            fireAction.action.performed -= FireLaser;
            fireAction.action.Disable();
        }
    }

    void FireLaser(InputAction.CallbackContext context)
    {
        if (projectilePrefab == null || muzzleTransform == null)
        {
            Debug.LogWarning("Projectile Prefab or Muzzle Transform not assigned!");
            return;
        }

        GameObject laser = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = muzzleTransform.forward * projectileSpeed;
        }

        Destroy(laser, 3f); // Fallback in case it doesn't hit anything
    }
}