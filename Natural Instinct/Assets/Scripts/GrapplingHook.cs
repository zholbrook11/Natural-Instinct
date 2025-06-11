using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public Transform playerBody;               // The object to pull (usually the rig or camera root)
    public LineRenderer lineRenderer;
    public LayerMask grappleLayer;
    public float maxDistance = 30f;

    private Vector3 grapplePoint;
    private SpringJoint joint;
    private bool isGrappling = false;

    public void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, grappleLayer))
        {
            grapplePoint = hit.point;

            Debug.DrawLine(transform.position, grapplePoint, Color.red, 2f);

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
