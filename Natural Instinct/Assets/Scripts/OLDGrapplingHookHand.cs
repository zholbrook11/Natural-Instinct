using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHookHand : MonoBehaviour
{
    [Header("Grapple Setup")]
    public Transform playerBody;
    public LayerMask grappleLayer;
    public float maxDistance = 30f;
    public float launchForce = 15f;
    public float gravityRestoreTime = 1.5f;
    public InputActionReference grappleAction;

    [Header("Aim Assist")]
    public float aimAssistAngle = 10f;
    public float aimAssistRadius = 2f;
    public GameObject aimConeVisual;

    [Header("Highlighting")]
    public Material highlightMaterial;

    [Header("Audio")] 
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip attachSound;
    public AudioClip releaseSound;

    [Header("Haptics")]
    public XRBaseController controller;
    public float hapticAmplitude = 0.4f;
    public float hapticDuration = 0.1f;

    private Vector3 grapplePoint;
    private LineRenderer lineRenderer;
    private CharacterController characterController;
    private Vector3 fallingVelocity;
    private float gravityTimer = 0f;
    private bool isGrappling = false;
    private Renderer lastHitRenderer;
    private Material originalMaterial;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        characterController = playerBody.GetComponent<CharacterController>();
        lineRenderer.positionCount = 0;
    }

    private void OnEnable()
    {
        if (grappleAction?.action != null)
        {
            grappleAction.action.Enable();
            grappleAction.action.performed += ctx => StartGrapple();
            grappleAction.action.canceled += ctx => StopGrapple();
        }
    }

    private void OnDisable()
    {
        if (grappleAction?.action != null)
        {
            grappleAction.action.performed -= ctx => StartGrapple();
            grappleAction.action.canceled -= ctx => StopGrapple();
            grappleAction.action.Disable();
        }
    }

    private void Update()
    {
        AimAssistHighlight();

        if (isGrappling)
        {
            Vector3 toGrapple = (grapplePoint - playerBody.position).normalized;
            characterController.Move(toGrapple * launchForce * Time.deltaTime);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
        else if (!characterController.isGrounded)
        {
            gravityTimer += Time.deltaTime;
            float gravityMultiplier = Mathf.Clamp01(gravityTimer / gravityRestoreTime);
            fallingVelocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
            characterController.Move(fallingVelocity * Time.deltaTime);
        }
        else
        {
            gravityTimer = 0f;
            fallingVelocity = Vector3.zero;
        }
    }

    private void AimAssistHighlight()
    {
        ClearHighlight();

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, aimAssistRadius, transform.forward, maxDistance, grappleLayer);

        foreach (RaycastHit hit in hits)
        {
            Vector3 toTarget = hit.point - transform.position;
            if (Vector3.Angle(transform.forward, toTarget) <= aimAssistAngle)
            {
                grapplePoint = hit.point;

                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    lastHitRenderer = renderer;
                    originalMaterial = renderer.sharedMaterial;
                    renderer.material = highlightMaterial;
                }
                return;
            }
        }
    }

    private void ClearHighlight()
    {
        if (lastHitRenderer != null)
        {
            lastHitRenderer.material = originalMaterial;
            lastHitRenderer = null;
        }
    }

    public void StartGrapple()
    {
        if (grapplePoint == Vector3.zero) return;

        isGrappling = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);

        if (audioSource && fireSound) audioSource.PlayOneShot(fireSound);
        if (audioSource && attachSound) audioSource.PlayOneShot(attachSound);
        if (controller != null) controller.SendHapticImpulse(hapticAmplitude, hapticDuration);

        // Reset gravity fall
        gravityTimer = 0f;
        fallingVelocity = Vector3.zero;
    }

    public void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;
        if (audioSource && releaseSound) audioSource.PlayOneShot(releaseSound);
        if (controller != null) controller.SendHapticImpulse(hapticAmplitude * 0.5f, hapticDuration * 0.5f);
    }
}