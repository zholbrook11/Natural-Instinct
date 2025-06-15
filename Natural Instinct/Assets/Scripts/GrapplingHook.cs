using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public Transform playerBody;
    public LineRenderer lineRenderer;
    public LayerMask grappleLayer;
    public float maxDistance = 30f;
    public float swingForce = 20f;
    public Material highlightMaterial;

    private Vector3 grapplePoint;
    private SpringJoint joint;
    private bool isGrappling = false;

    private Renderer lastHitRenderer;
    private Material originalMaterial;

    void Update()
    {
        HighlightGrappleTarget();

        if (isGrappling && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            ApplySwingForce(); // 🔥 New force logic
        }
    }

    /*void ApplySwingForce()
    {
        if (playerBody == null || joint == null) return;

        Rigidbody rb = playerBody.GetComponent<Rigidbody>();

        // Use the direction the player/cube is facing for better control
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float horizontalInput = Input.GetAxis("Horizontal"); // A/D
        float verticalInput = Input.GetAxis("Vertical");     // W/S

        Vector3 moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            rb.AddForce(moveDirection * swingForce, ForceMode.Acceleration);

            // Optional: visualize applied force direction
            Debug.DrawRay(playerBody.position, moveDirection * 3f, Color.green, 0.1f);
        }
    }*/
    void ApplySwingForce()
{
    if (playerBody == null || joint == null) return;

    Rigidbody rb = playerBody.GetComponent<Rigidbody>();
    if (rb == null) return;

    Vector3 toGrapple = grapplePoint - playerBody.position;
    Vector3 swingDirection = Vector3.Cross(toGrapple.normalized, Vector3.up).normalized;

    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    Vector3 inputForce = swingDirection * horizontalInput + transform.forward * verticalInput;
    rb.AddForce(inputForce * swingForce, ForceMode.Acceleration);
}



    void HighlightGrappleTarget()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, grappleLayer))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (renderer != lastHitRenderer)
                {
                    ClearLastHighlight();

                    lastHitRenderer = renderer;
                    originalMaterial = renderer.material;
                    renderer.material = highlightMaterial;
                }
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

    public void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, grappleLayer))
        {
            grapplePoint = hit.point;

            Debug.DrawLine(transform.position, grapplePoint, Color.red, 2f);

            Rigidbody rb = playerBody.GetComponent<Rigidbody>();
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            
            joint = playerBody.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(playerBody.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, grapplePoint);
            }

            isGrappling = true;

            // Launch the player slightly toward the grapple point
            Vector3 directionToPoint = (grapplePoint - playerBody.position).normalized;
            rb.linearVelocity = directionToPoint * 10f; // Tune this value as needed

            if (rb != null) rb.useGravity = false;
        }
    }

    public void StopGrapple()
    {
        if (joint != null)
        {
            Destroy(joint);
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }

        isGrappling = false;

        // Re-enable gravity
        Rigidbody rb = playerBody.GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = true;
    }

    private void LateUpdate()
    {
        if (!isGrappling) return;

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }
}
