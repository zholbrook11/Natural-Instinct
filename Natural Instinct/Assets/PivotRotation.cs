using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    public Transform rotationPivot; // Assign this in the Inspector
    public float rotationSpeed = 90f; // Degrees per second

    private bool isRotating = false;

    public void RotateFace(List<Transform> cubesToRotate, Vector3 axis, float angle)
    {
        if (!isRotating)
            StartCoroutine(RotateCoroutine(cubesToRotate, axis, angle));
    }

    private IEnumerator RotateCoroutine(List<Transform> cubes, Vector3 axis, float angle)
    {
        if (cubes == null || cubes.Count == 0)
        {
            Debug.LogError("No cubes provided for rotation!");
            yield break;
        }

        isRotating = true;

        // 1. Calculate the average center
        Vector3 center = Vector3.zero;
        foreach (var cube in cubes)
            center += cube.position;
        center /= cubes.Count;

        // 2. Move pivot to center
        rotationPivot.position = center;

        // 3. Parent cubes to pivot
        foreach (var cube in cubes)
            cube.SetParent(rotationPivot);

        // 4. Rotate the pivot over time
        float rotated = 0f;
        while (rotated < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (rotated + step > angle) step = angle - rotated;

            rotationPivot.Rotate(axis, step, Space.World);
            rotated += step;

            yield return null;
        }

        // 5. Unparent and reset
        foreach (var cube in cubes)
            cube.SetParent(null);

        rotationPivot.rotation = Quaternion.identity;
        isRotating = false;