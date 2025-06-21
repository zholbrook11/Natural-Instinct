using System.Collections.Generic;
using UnityEngine;

public class RubiksController : MonoBehaviour
{
    public CubeFaceRotator rotator;       // Reference to CubeFaceRotator script
    public List<Transform> allCubes;      // List of the 8 cube pieces
    public Transform cubeCenter;          // Center of the Rubik's cube
    public Transform cubeRoot;            // Drag in the root GameObject

    void Update()
    {
        float angle = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -90f : 90f;

        if (Input.GetKeyDown(KeyCode.R)) RotateFace(Vector3.forward, angle);  // Front
        if (Input.GetKeyDown(KeyCode.F)) RotateFace(Vector3.back, angle);     // Back
        if (Input.GetKeyDown(KeyCode.U)) RotateFace(Vector3.up, angle);       // Top
        if (Input.GetKeyDown(KeyCode.D)) RotateFace(Vector3.down, angle);     // Bottom
        if (Input.GetKeyDown(KeyCode.L)) RotateFace(Vector3.left, angle);     // Left
        if (Input.GetKeyDown(KeyCode.T)) RotateFace(Vector3.right, angle);    // Right
    }


    void RotateFace(Vector3 faceDirection, float angle)
    {
        var cubes = GetCubesOnFace(faceDirection, 0.4f);
        if (cubes.Count > 0)
        {
            // ✅ Transform the direction into world space relative to cubeRoot
            Vector3 worldAxis = cubeRoot.TransformDirection(faceDirection);
            rotator.RotateFace(cubes, worldAxis, angle, null);
        }
        else
        {
            Debug.LogWarning($"No cubes found on face {faceDirection}!");
        }
    }



    public List<Transform> GetCubesOnFace(Vector3 faceDirection, float threshold = 0.1f)
    {
        List<Transform> result = new List<Transform>();

        Vector3 center = cubeCenter.position;

        // ✅ Convert local direction to world direction
        Vector3 worldFaceDir = cubeRoot.TransformDirection(faceDirection.normalized);

        foreach (Transform cube in allCubes)
        {
            Vector3 local = cube.position - center;

            float dot = Vector3.Dot(local.normalized, worldFaceDir);

            if (dot > threshold)
            {
                result.Add(cube);
            }
        }

        Debug.Log($"Found {result.Count} cubes on face {faceDirection}");
        return result;
    }

}
