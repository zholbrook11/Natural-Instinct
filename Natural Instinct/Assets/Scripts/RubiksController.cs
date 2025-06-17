using System.Collections.Generic;
using UnityEngine;

public class RubiksController : MonoBehaviour
{
    public CubeFaceRotator rotator;       // Reference to CubeFaceRotator script
    public List<Transform> allCubes;      // List of the 8 cube pieces
    public Transform cubeCenter;          // Center of the Rubik's cube

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RotateFace(Vector3.forward);     // Front
        if (Input.GetKeyDown(KeyCode.F)) RotateFace(Vector3.back);        // Back
        if (Input.GetKeyDown(KeyCode.U)) RotateFace(Vector3.up);          // Top
        if (Input.GetKeyDown(KeyCode.D)) RotateFace(Vector3.down);        // Bottom
        if (Input.GetKeyDown(KeyCode.L)) RotateFace(Vector3.left);        // Left
        if (Input.GetKeyDown(KeyCode.T)) RotateFace(Vector3.right);       // Right
    }

    void RotateFace(Vector3 faceDirection)
    {
        var cubes = GetCubesOnFace(faceDirection, 0.4f);

        if (cubes.Count > 0)
        {
            rotator.RotateFace(cubes, faceDirection, 90f);
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

        foreach (Transform cube in allCubes)
        {
            Vector3 local = cube.position - center;

            // Dot product tells us how aligned the cube is with the face direction
            float dot = Vector3.Dot(local.normalized, faceDirection.normalized);

            if (dot > threshold)
            {
                result.Add(cube);
            }
        }

        Debug.Log($"Found {result.Count} cubes on face {faceDirection}");
        return result;
    }
}
