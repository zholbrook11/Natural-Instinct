using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

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
    public float pullSpeed = 15f;

    [Header("Tether Mode")]
    public bool useTetherGrappleMode = true;
    public float tetherStopDistance = 2f;
    public float tetherPullAcceleration = 30f;
    public float tetherMaxPullSpeed = 20f;
    public float tetherOrbitSpeed = 3f;

    [Header("Aim Assist")]
    public float aimAssistAngle = 10f;
    public float aimAssistRadius = 2f;
    public GameObject aimConeVisual;

    [Header("Highlighting")]
    public Material highlightMaterial;
    public Material dualHighlightMaterial;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip attachSound;
    public AudioClip releaseSound;

    [Header("Haptics")]
    public XRBaseController controller;
    public float hapticAmplitude = 0.4f;
    public float hapticDuration = 0.1f;

    [Header("Settings")]
    public bool enableAutoHighlight = true;

    private Vector3 grapplePoint;
    private LineRenderer lineRenderer;
    private CharacterController characterController;
    private Vector3 fallingVelocity;
    private float gravityTimer = 0f;
    private bool isGrappling = false;

    private Renderer currentTargetRenderer;

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

        ClearHighlight();
    }

    private void Update()
    {
        UpdateHighlight();

        if (useTetherGrappleMode && isGrappling)
            return;

        if (isGrappling && !useTetherGrappleMode)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            Vector3 direction = (grapplePoint - playerBody.position).normalized;
            Vector3 move = direction * pullSpeed * Time.deltaTime;
            characterController.Move(move);
        }
        else if (!isGrappling && !characterController.isGrounded)
        {
            gravityTimer += Time.deltaTime;
            float gravityMultiplier = Mathf.Clamp01(gravityTimer / gravityRestoreTime);
            fallingVelocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
            characterController.Move(fallingVelocity * Time.deltaTime);
        }
        else if (characterController.isGrounded)
        {
            gravityTimer = 0f;
            fallingVelocity = Vector3.zero;
        }
    }

    private void UpdateHighlight()
    {
        if (!enableAutoHighlight || isGrappling) return;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, aimAssistRadius, transform.forward, maxDistance, grappleLayer);
        Renderer bestRenderer = null;
        float closestAngle = aimAssistAngle;

        foreach (RaycastHit hit in hits)
        {
            Vector3 toTarget = hit.point - transform.position;
            float angle = Vector3.Angle(transform.forward, toTarget);

            if (angle <= closestAngle)
            {
                closestAngle = angle;
                bestRenderer = hit.collider.GetComponent<Renderer>();
                grapplePoint = hit.point;
            }
        }

        if (bestRenderer != currentTargetRenderer)
        {
            if (currentTargetRenderer != null)
                HighlightManager.Instance.RemoveHighlight(currentTargetRenderer);

            if (bestRenderer != null)
                HighlightManager.Instance.AddHighlight(bestRenderer);

            currentTargetRenderer = bestRenderer;
        }

        if (bestRenderer == null && currentTargetRenderer != null)
        {
            HighlightManager.Instance.RemoveHighlight(currentTargetRenderer);
            currentTargetRenderer = null;
        }
    }

    private void ClearHighlight()
    {
        if (currentTargetRenderer != null)
        {
            HighlightManager.Instance.RemoveHighlight(currentTargetRenderer);
            currentTargetRenderer = null;
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

        gravityTimer = 0f;
        fallingVelocity = Vector3.zero;

        if (useTetherGrappleMode)
            StartCoroutine(TetherGrappleRoutine());
    }

    public void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;

        if (audioSource && releaseSound) audioSource.PlayOneShot(releaseSound);
        if (controller != null) controller.SendHapticImpulse(hapticAmplitude * 0.5f, hapticDuration * 0.5f);

        if (useTetherGrappleMode)
        {
            Vector3 releaseDir = (playerBody.transform.position - grapplePoint).normalized;
            float speed = characterController.velocity.magnitude;
            StartCoroutine(ApplyReleaseMomentum(releaseDir, speed));
        }
    }

    private IEnumerator TetherGrappleRoutine()
    {
        while (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            Vector3 toGrapple = grapplePoint - playerBody.position;
            float distance = toGrapple.magnitude;

            // Pull to tether distance
            if (distance > tetherStopDistance)
            {
                float pullSpeed = Mathf.Min(tetherMaxPullSpeed, (distance - tetherStopDistance) * tetherPullAcceleration);
                Vector3 pullVelocity = toGrapple.normalized * pullSpeed;
                characterController.Move(pullVelocity * Time.deltaTime);
            }

            // Dynamic control scaling as you near the tether
            float leashPercent = Mathf.InverseLerp(0.5f * tetherStopDistance, tetherStopDistance, distance);
            Vector3 aimDirection = transform.forward.normalized;
            Vector3 aimMovement = aimDirection * tetherOrbitSpeed * leashPercent;
            characterController.Move(aimMovement * Time.deltaTime);

            // Smooth tether radius correction
            Vector3 fromAnchor = playerBody.position - grapplePoint;
            if (fromAnchor.magnitude > tetherStopDistance)
            {
                Vector3 clampedPosition = grapplePoint + fromAnchor.normalized * tetherStopDistance;
                Vector3 correction = clampedPosition - playerBody.position;
                characterController.Move(correction * 0.25f); // Smooth correction
            }

            yield return null;
        }
    }

    private IEnumerator ApplyReleaseMomentum(Vector3 direction, float speed)
    {
        float gravity = 9.81f;

        while (!characterController.isGrounded)
        {
            Vector3 motion = direction * speed;
            motion.y -= gravity * Time.deltaTime;

            characterController.Move(motion * Time.deltaTime);
            speed = Mathf.Lerp(speed, 0f, Time.deltaTime * 0.5f);

            yield return null;
        }
    }
}
