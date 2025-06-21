using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    void Unlock()
    {
        isLocked = false;
        grabInteractable.enabled = true;
    }
}
