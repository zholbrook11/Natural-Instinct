using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
    public CubeFaceRotator rotator;
    public List<Transform> frontFaceCubes;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotator.RotateFace(frontFaceCubes, Vector3.forward, 90f);
        }
    }
}
