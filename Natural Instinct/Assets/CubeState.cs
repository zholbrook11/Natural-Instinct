using System.Collections.Generic;
using UnityEngine;

public class CubeState : MonoBehaviour
{
    // Each face holds the 4 visible face GameObjects
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();

    // List of all sides for easy access
    public List<List<GameObject>> allSides = new List<List<GameObject>>();

    // Holds the cubelets currently being rotated
    public List<Transform> rotatingCubelets = new List<Transform>();

    // A reference point for rotation
    public Transform pivot;

    void Start()
    {
        // Add all six sides to one list for convenience
        allSides.Add(up);
        allSides.Add(down);
        allSides.Add(left);
        allSides.Add(right);
        allSides.Add(front);
        allSides.Add(back);
    }

    public void PickUp(List<GameObject> cubeSide)
    {
        rotatingCubelets.Clear();

        foreach (GameObject face in cubeSide)
        {
            // Attach the parent of each face
            Transform cubelet = face.transform.parent; // Face is a child of the cubelet
            if (!rotatingCubelets.Contains(cubelet))
            {
                rotatingCubelets.Add(cubelet);
            }
        }

        // Make all cubelets children of the pivot so they rotate together
        foreach (Transform cubelet in rotatingCubelets)
        {
            cubelet.SetParent(pivot);
        }
    }

    public void PutDown()
    {
        // Detach cubelets from pivot after rotation
        foreach (Transform cubelet in rotatingCubelets)
        {
            cubelet.SetParent(null);
        }

        rotatingCubelets.Clear();
    }
}
