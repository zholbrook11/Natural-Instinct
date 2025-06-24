using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RevealOnGrabAndGravity : MonoBehaviour
{
    public GameObject caveExit;
    public AudioClip grabSound;

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private AudioSource audioSource;

    // NEW: link to spawner
    private DroneWaveSpawner spawner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Find the spawner in the scene (or reference it manually if needed)
        spawner = FindObjectOfType<DroneWaveSpawner>();
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

        if (caveExit != null)
            caveExit.SetActive(true);

        if (grabSound != null)
            audioSource.PlayOneShot(grabSound);

        // Notify spawner
        if (spawner != null)
            spawner.OnTesseractGrabbed();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        rb.useGravity = true;
    }
}
