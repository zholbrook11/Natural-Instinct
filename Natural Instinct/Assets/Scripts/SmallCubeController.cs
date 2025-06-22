using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;


public class SmallCubeController : MonoBehaviour
{
    public CubeFaceRotator bigCubeRotator;     // Script on large cube root
    public Transform cubeRoot;                 // Root Transform of small cube
    public Transform returnPosition;           // Hover position above pedestal
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    [HideInInspector] public bool isLocked;

    private bool shouldReturn = false;
    public float returnSpeed = 2f;

    void Update()
    {
        if (shouldReturn)
        {
            cubeRoot.position = Vector3.Lerp(cubeRoot.position, returnPosition.position, Time.deltaTime * returnSpeed);
            cubeRoot.rotation = Quaternion.Slerp(cubeRoot.rotation, returnPosition.rotation, Time.deltaTime * returnSpeed);
            if (Vector3.Distance(cubeRoot.position, returnPosition.position) < 0.01f)
                shouldReturn = false;
        }
    }

    public void OnReleased()
    {
        if (!isLocked)
        {
            shouldReturn = true;
            Rigidbody rb = cubeRoot.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
        }
    }

    public void OnPickedUp()
    {
        shouldReturn = false;
        Rigidbody rb = cubeRoot.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;
    }

    public void OnFaceTwist(List<Transform> faceCubes, Vector3 worldAxis, float angle)
    {
        if (isLocked) return;
        isLocked = true;
        grabInteractable.enabled = false;

        bigCubeRotator.RotateFace(
            cubesToRotate: faceCubes,
            axis: worldAxis,
            angle: angle,
            callback: Unlock
        );
    }

    public List<Transform> GetCubesOnFace(Vector3 faceDirection, float threshold = 0.5f)
    {
        List<Transform> result = new List<Transform>();

        Vector3 center = cubeRoot.position;
        Vector3 worldFaceDir = cubeRoot.TransformDirection(faceDirection.normalized);

        foreach (Transform cube in cubeRoot)
        {
            if (!cube.name.StartsWith("Piece")) continue;

            Vector3 local = cube.position - center;
            float dot = Vector3.Dot(local.normalized, worldFaceDir);

            Debug.Log($"Checking {cube.name}: dot = {dot}");

            if (Mathf.Abs(dot) > threshold && dot > 0)
            {
                Debug.Log($"✔ Added {cube.name} to face");
                result.Add(cube);
            }
        }

        Debug.Log($"[FaceGrab] Found {result.Count} cubes on face: {faceDirection}");
        return result;
    }

    public void RotateFace(Vector3 faceDirection, float angle)
    {
        if (isLocked) return;

        isLocked = true;
        grabInteractable.enabled = false;

        var faceCubes = GetCubesOnFace(faceDirection, 0.1f); // Use tighter threshold for 2x2
        Vector3 worldAxis = cubeRoot.TransformDirection(faceDirection.normalized);

        bigCubeRotator.RotateFace(faceCubes, worldAxis, angle, Unlock);
    }




    void Unlock()
    {
        isLocked = false;
        grabInteractable.enabled = true;
    }

    void Awake()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(args => OnPickedUp());
            grabInteractable.selectExited.AddListener(args => OnReleased());
        }
    }

}
