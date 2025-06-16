using UnityEngine;

public class GrappleSwingController : MonoBehaviour
{
    [Header("References")]
    public CharacterController characterController;
    public Transform playerHead; // Typically XR Camera
    public Transform leftHand, rightHand;

    [Header("Swing Settings")]
    public float gravity = 30f;
    public float ropeTension = 40f;
    public float swingDamp = 1f;
    public float maxRopeLength = 25f;
    public float minRopeLength = 2f;
    public float ropeShortenSpeed = 10f;

    [Header("Debug")]
    public bool isSwingingLeft = false;
    public bool isSwingingRight = false;

    private Vector3 velocity;
    private Vector3? leftGrapplePoint = null;
    private Vector3? rightGrapplePoint = null;

    void Update()
    {
        SimulateSwing(Time.deltaTime);
    }

    public void StartGrapple(Vector3 grapplePoint, bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftGrapplePoint = grapplePoint;
            isSwingingLeft = true;
        }
        else
        {
            rightGrapplePoint = grapplePoint;
            isSwingingRight = true;
        }
    }

    public void StopGrapple(bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftGrapplePoint = null;
            isSwingingLeft = false;
        }
        else
        {
            rightGrapplePoint = null;
            isSwingingRight = false;
        }
    }

    void SimulateSwing(float deltaTime)
    {
        if (!isSwingingLeft && !isSwingingRight)
        {
            // Apply gravity if not swinging
            velocity.y -= gravity * deltaTime;
            characterController.Move(velocity * deltaTime);
            return;
        }

        Vector3 centerPoint = transform.position + playerHead.localPosition;
        Vector3 targetTensionForce = Vector3.zero;

        // Handle both hands (combined effect)
        if (isSwingingLeft && leftGrapplePoint.HasValue)
        {
            targetTensionForce += CalculateRopeTension(centerPoint, leftGrapplePoint.Value);
        }

        if (isSwingingRight && rightGrapplePoint.HasValue)
        {
            targetTensionForce += CalculateRopeTension(centerPoint, rightGrapplePoint.Value);
        }

        // Apply gravity
        velocity += Vector3.down * gravity * deltaTime;

        // Apply rope tension
        velocity += targetTensionForce * deltaTime;

        // Apply damping
        velocity *= (1f - swingDamp * deltaTime);

        characterController.Move(velocity * deltaTime);
    }

    Vector3 CalculateRopeTension(Vector3 playerPos, Vector3 grapplePoint)
    {
        Vector3 dirToAnchor = grapplePoint - playerPos;
        float distance = dirToAnchor.magnitude;

        if (distance < minRopeLength)
            return Vector3.zero;

        float tensionFactor = Mathf.Clamp01((distance - minRopeLength) / (maxRopeLength - minRopeLength));
        return dirToAnchor.normalized * tensionFactor * ropeTension;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

}
