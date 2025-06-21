using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
public class SmallCubeFaceInteractor : MonoBehaviour
{
    public Vector3 faceLocalDirection;
    public Transform facePiece;
    public float twistThreshold = 70f;
    public float snapAngle = 90f;
    public SmallCubeController smallCubeController;

    UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor grabbingInteractor;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable grabInteractable;
    float initialAngle;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabStart);
        grabInteractable.selectExited.AddListener(OnGrabEnd);
    }

    void OnGrabStart(SelectEnterEventArgs args)
    {
        grabbingInteractor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
        initialAngle = facePiece.localRotation.eulerAngles.y;
    }

    void OnGrabEnd(SelectExitEventArgs args)
    {
        grabbingInteractor = null;
    }

    void Update()
    {
        if (grabbingInteractor == null) return;

        Vector3 handDir = grabbingInteractor.transform.rotation * Vector3.forward;
        float angle = Vector3.SignedAngle(transform.up, handDir, transform.forward);
        float delta = Mathf.DeltaAngle(initialAngle, angle);

        facePiece.localRotation = Quaternion.AngleAxis(initialAngle + delta, faceLocalDirection);

        if (Mathf.Abs(delta) >= twistThreshold)
        {
            float direction = Mathf.Sign(delta);
            facePiece.localRotation = Quaternion.AngleAxis(snapAngle * direction, faceLocalDirection);

            grabbingInteractor = null; // stops further movement
            smallCubeController.OnFaceTwist(
            null,
            transform.TransformDirection(faceLocalDirection),
            snapAngle * direction
            );
        }
    }
}
