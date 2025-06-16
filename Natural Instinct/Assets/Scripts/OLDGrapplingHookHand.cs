using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHookHand : MonoBehaviour
{
    public Transform playerBody;
    public LineRenderer lineRenderer;
    public LayerMask grappleLayer;
    public float maxDistance = 30f;
    public float pullSpeed = 10f;
    [SerializeField] public InputActionReference grappleAction;
    public Material highlightMaterial;


    private Renderer lastHitRenderer;
    private Material originalMaterial;
    private Vector3 grapplePoint;
    private bool isGrappling = false;

    private void OnEnable()
    {
        if (grappleAction.action != null)
        {
            grappleAction.action.Enable();
            grappleAction.action.performed += ctx => StartGrapple();
            grappleAction.action.canceled += ctx => StopGrapple();
        }
    }

    private void OnDisable()
    {
        if (grappleAction.action != null)
        {
            grappleAction.action.Disable();
            grappleAction.action.performed -= ctx => StartGrapple();
            grappleAction.action.canceled -= ctx => StopGrapple();
        }
    }

    void Update()
{
    HighlightGrappleTarget();

    if (isGrappling)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);

        Vector3 toGrapple = grapplePoint - playerBody.position;
        Vector3 swingDir = Vector3.Cross(toGrapple.normalized, Vector3.up).normalized;

        // Apply movement (optional: based on input)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputMovement = (swingDir * horizontalInput + transform.forward * verticalInput) * pullSpeed * Time.deltaTime;

        // Pull towards grapple point
        Vector3 pull = toGrapple.normalized * (pullSpeed * 0.5f * Time.deltaTime);

        playerBody.GetComponent<CharacterController>().Move(inputMovement + pull);
    }
}


    void StartGrapple()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            lineRenderer.positionCount = 2;
            isGrappling = true;
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;
    }
    void HighlightGrappleTarget()
{
    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, grappleLayer))
    {
        Renderer renderer = hit.collider.GetComponent<Renderer>();
        if (renderer != null && renderer != lastHitRenderer)
        {
            ClearLastHighlight();

            lastHitRenderer = renderer;
            originalMaterial = renderer.material;
            renderer.material = highlightMaterial;
        }
    }
    else
    {
        ClearLastHighlight();
    }
}

    void ClearLastHighlight()
    {
        if (lastHitRenderer != null)
        {
            lastHitRenderer.material = originalMaterial;
            lastHitRenderer = null;
        }
    }

}
