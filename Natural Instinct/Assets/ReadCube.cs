using System.Collections.Generic;
using UnityEngine;

public class ReadCube : MonoBehaviour
{
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    private List<GameObject> frontRays = new List<GameObject>();
    private List<GameObject> backRays = new List<GameObject>();
    private List<GameObject> upRays = new List<GameObject>();
    private List<GameObject> downRays = new List<GameObject>();
    private List<GameObject> leftRays = new List<GameObject>();
    private List<GameObject> rightRays = new List<GameObject>();

    private int layerMask = 1 << 3; // Make sure the cube faces are on Layer 3
    private CubeState cubeState;
    public GameObject emptyGo;

    void Start()
    {
        cubeState = FindFirstObjectByType<CubeState>();
        SetRayTransforms();
    }

    void Update()
    {
        ReadState();
    }

    public void ReadState()
    {
        if (cubeState == null)
        {
            cubeState = FindFirstObjectByType<CubeState>();
            if (cubeState == null)
            {
                Debug.LogError("CubeState not found in the scene.");
                return;
            }
        }

        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);
    }

    void SetRayTransforms()
    {
        upRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(270, 90, 0));
        leftRays = BuildRays(tLeft, new Vector3(0, -90, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 90, 0));
        frontRays = BuildRays(tFront, new Vector3(0, 180, 0));
        backRays = BuildRays(tBack, new Vector3(0, 0, 0));
    }

    List<GameObject> BuildRays(Transform rayTransform, Vector3 eulerRotation)
    {
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();

        rayTransform.localRotation = Quaternion.Euler(eulerRotation); // Fix: rotate before instantiating rays

        for (float y = 0.25f; y >= -0.25f; y -= 0.5f) // Fix: `y > -0.25` and `y -= -0.5f` is logically incorrect
        {
            for (float x = -0.25f; x <= 0.25f; x += 0.5f)
            {
                Vector3 offset = new Vector3(x, y, 0);
                Vector3 startPos = rayTransform.TransformPoint(offset);
                GameObject rayStart = Instantiate(emptyGo, startPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();
                rays.Add(rayStart);
                rayCount++;
            }
        }

        return rays;
    }

    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new List<GameObject>();

        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 origin = rayStart.transform.position;
            Vector3 direction = rayTransform.forward;
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
                facesHit.Add(hit.collider.gameObject);
                Debug.Log("Hit: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawRay(origin, direction * 1f, Color.red); // Shorter red ray for visualization
            }
        }

        return facesHit;
    }
}
