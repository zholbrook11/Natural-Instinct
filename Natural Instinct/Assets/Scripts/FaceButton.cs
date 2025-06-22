using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class FaceButton : MonoBehaviour
{
    public Vector3 faceDirection = Vector3.forward;
    public float rotationAngle = 90f;
    public AudioClip clickSound;
    public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.1f;

    private SmallCubeController cubeController;

    private Material originalMat;
    public Material highlightMat;

    private void Awake()
    {
        cubeController = GetComponentInParent<SmallCubeController>();

        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelected);
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);

        originalMat = GetComponent<Renderer>().material;
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        GetComponent<Renderer>().material = highlightMat;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        GetComponent<Renderer>().material = originalMat;
    }


    private void OnSelected(SelectEnterEventArgs args)
    {
        if (cubeController != null)
        {
            cubeController.RotateFace(faceDirection, rotationAngle);
        }

        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }

        // Haptics
        if (args.interactorObject.transform.TryGetComponent(out UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor))
        {
            controllerInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }
    }
}
