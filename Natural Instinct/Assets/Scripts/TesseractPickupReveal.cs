using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RevealOnGrabAndGravity : MonoBehaviour
{
    public GameObject caveExit;
    public AudioClip grabSound;
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        rb.useGravity = false;
        if (caveExit != null) caveExit.SetActive(true);
        if (grabSound != null) audioSource.PlayOneShot(grabSound);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        rb.useGravity = true;
    }
}