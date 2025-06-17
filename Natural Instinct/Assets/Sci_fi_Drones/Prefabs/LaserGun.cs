using UnityEngine;
using System.Collections;

public class LaserGun : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private LineRenderer beam;
    [SerializeField] private float beamDuration = 0.05f;
    [SerializeField] private float fireRate     = 8f;     // shots per second
    [SerializeField] private float range        = 100f;
    [SerializeField] private LayerMask targetMask;
    [Tooltip("Optional Transform at the muzzle. Leave null to shoot from camera")]
    [SerializeField] private Transform muzzle;

    float nextFireTime;
    Camera cam;            // cache for speed

    void Awake()
    {
        cam = Camera.main;
        beam.useWorldSpace = true;          // <-- IMPORTANT!
        beam.enabled = false;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    /* ------------------ core logic ------------------ */
    void Fire()
    {
        // 1. Ray that always starts at centre of screen
        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));

        // 2. Default end-point is "miss" at max range
        Vector3 hitPoint = ray.origin + ray.direction * range;

        // 3. If we hit something on Target layer, deal damage & shorten beam
        if (Physics.Raycast(ray, out RaycastHit hit, range, targetMask))
        {
            hit.collider.GetComponentInParent<DroneHealth>()?.TakeDamage(1);
            hitPoint = hit.point;
        }

        // 4. Where does the beam START? (camera or custom muzzle)
        Vector3 startPoint = muzzle ? muzzle.position : ray.origin;

        // 5. Draw the beam as a coroutine so it auto-disables
        StartCoroutine(ShowBeam(startPoint, hitPoint));
    }

    IEnumerator ShowBeam(Vector3 start, Vector3 end)
    {
        beam.SetPosition(0, start);
        beam.SetPosition(1, end);
        beam.enabled = true;
        yield return new WaitForSeconds(beamDuration);
        beam.enabled = false;
    }
}
