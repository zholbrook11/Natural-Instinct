using UnityEngine;

public class SatelliteFullOrbitControl : MonoBehaviour
{
    [Header("Orbit Center")]
    public Transform orbitCenter; // Object to orbit around

    [Header("Orbit Control")]
    public float orbitRadius = 50f;
    public Vector3 orbitAxis = Vector3.up; // Axis to orbit around
    public bool useAutomaticOrbit = true;
    public float orbitSpeed = 10f; // Degrees per second

    [Header("Manual Angle Control")]
    [Range(0f, 360f)]
    public float manualAngle = 0f; // Only used if useAutomaticOrbit is false

    [Header("Offset")]
    public Vector3 satelliteOffset = Vector3.zero; // Additional positional offset

    private float currentAngle = 0f;

    void Update()
    {
        if (orbitCenter == null) return;

        // Update angle
        if (useAutomaticOrbit)
        {
            currentAngle += orbitSpeed * Time.deltaTime;
            currentAngle %= 360f;
        }
        else
        {
            currentAngle = manualAngle % 360f;
        }

        // Calculate orbit rotation
        Quaternion orbitRotation = Quaternion.AngleAxis(currentAngle, orbitAxis.normalized);
        Vector3 orbitDirection = orbitRotation * Vector3.forward;

        // Set position
        Vector3 orbitPosition = orbitCenter.position + orbitDirection * orbitRadius + satelliteOffset;
        transform.position = orbitPosition;

        // Optional: face the center of orbit
        transform.LookAt(orbitCenter.position);
    }

    /// <summary>
    /// Call this to manually set the orbit angle (0-360)
    /// </summary>
    public void SetManualAngle(float angle)
    {
        manualAngle = angle;
        useAutomaticOrbit = false;
    }

    /// <summary>
    /// Call this to switch back to automatic orbit
    /// </summary>
    public void EnableAutoOrbit()
    {
        useAutomaticOrbit = true;
    }
}
