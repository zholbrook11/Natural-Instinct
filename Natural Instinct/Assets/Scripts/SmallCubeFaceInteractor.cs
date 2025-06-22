using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class SmallCubeFaceInteractor : MonoBehaviour
{
    public Vector3 faceLocalDirection;
    public Transform facePiece; // Optional, for feedback
    public float twistThreshold = 70f;
    public float snapAngle = 90f;
    public SmallCubeController smallCubeController;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor grabbingInteractor;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private float initialAngle;

    private List<Transform> faceCubes; // CUBES ON THIS FACE

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabStart);
        grabInteractable.selectExited.AddListener(OnGrabEnd);

        FindFaceCubes();
    }

    void FindFaceCubes()
    {
        /*faceCubes = new List<Transform>();

        Vector3 worldFaceDirection = transform.TransformDirection(faceLocalDirection).normalized;
        Vector3 faceCenter = transform.position;

        foreach (Transform child in smallCubeController.cubeRoot)
        {
            float distance = Mathf.Abs(Vector3.Dot(worldFaceDirection, (child.position - faceCenter)));
            if (distance < 0.1f) // Try increasing to 0.2 if no matches
            {
                faceCubes.Add(child);
                Debug.Log("✔ Face cube found: " + child.name);
            }
        }

        Debug.Log($"✅ Total cubes found on face: {faceCubes.Count}");
        */
        faceCubes = smallCubeController.GetCubesOnFace(faceLocalDirection);
        Debug.Log($"[FaceGrab] Found {faceCubes.Count} cubes on face: {faceLocalDirection}");
        Debug.Log($"[FaceGrab] Normalized world direction: {transform.TransformDirection(faceLocalDirection.normalized)}");




    }


    void OnGrabStart(SelectEnterEventArgs args)
    {
        grabbingInteractor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
        Vector3 handDir = args.interactorObject.transform.rotation * Vector3.forward;
        Vector3 faceNormal = transform.TransformDirection(faceLocalDirection);
        initialAngle = Vector3.SignedAngle(transform.up, handDir, faceNormal);

    }

    void OnGrabEnd(SelectExitEventArgs args)
    {
        grabbingInteractor = null;
    }

    void Update()
    {
        if (grabbingInteractor == null || faceCubes == null || faceCubes.Count == 0)
            return;

        Vector3 handDir = grabbingInteractor.transform.rotation * Vector3.forward;
        Vector3 faceNormal = transform.TransformDirection(faceLocalDirection);
        float currentAngle = Vector3.SignedAngle(transform.up, handDir, faceNormal);

        float delta = Mathf.DeltaAngle(initialAngle, currentAngle);

        if (facePiece != null)
            facePiece.localRotation = Quaternion.AngleAxis(initialAngle + delta, faceLocalDirection);

        if (Mathf.Abs(delta) >= twistThreshold)
        {
            float direction = Mathf.Sign(delta);
            smallCubeController.OnFaceTwist(
                faceCubes,
                faceNormal,
                snapAngle * direction
            );

            grabbingInteractor = null; // Prevent multiple triggers
        }
    }

}
