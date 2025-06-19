using System.Collections.Generic;
using UnityEngine;

public class SelectFace : MonoBehaviour
{
    private CubeState cubeState;
    private ReadCube readCube;
    private PivotRotation pivotRotation;
    private int layerMask = 1 << 3;

    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        pivotRotation = FindObjectOfType<PivotRotation>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            readCube.ReadState();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
            {
                GameObject face = hit.collider.gameObject;

                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back
                };

                foreach (List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        Debug.Log("Face selected: " + face.name);
                        pivotRotation.Rotate(cubeSide);
                        break;
                    }
                }
            }
        }
    }
}
