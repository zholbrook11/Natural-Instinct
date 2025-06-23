using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TesseractAimVibration : MonoBehaviour
{
    public Transform target;
    public float maxShakeStrength = 0.5f;
    public float maxDistance = 10f;
    //public HapticImpulsePlayer hapticPlayer;
    public AudioClip aimSound;

    private Vector3 originalLocalPosition;
    private AudioSource audioSource;
    private float lastPlayTime = 0f;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Assign the pedestal target by finding it in the scene
        GameObject pedestal = GameObject.Find("Pedestal");
        if (pedestal != null)
        {
            Transform snapPoint = pedestal.transform.Find("SnapPoint");
            target = snapPoint != null ? snapPoint : pedestal.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 forward = transform.forward;

        float alignment = Vector3.Dot(forward, toTarget);
        alignment = Mathf.Clamp01(alignment);

        float distance = Vector3.Distance(transform.position, target.position);
        float distanceFactor = Mathf.Clamp01(1f - (distance / maxDistance));

        float intensity = alignment * distanceFactor * maxShakeStrength;

        transform.localPosition = originalLocalPosition;

        if (intensity > 0.01f)
        {
            transform.localPosition += Random.insideUnitSphere * intensity * 0.01f;

            //if (hapticPlayer != null && intensity > 0.05f)
             //   hapticPlayer.SendHapticImpulse(intensity, 0.1f);

            if (aimSound != null && Time.time - lastPlayTime > 1f)
            {
                audioSource.PlayOneShot(aimSound);
                lastPlayTime = Time.time;
            }
        }
    }
}