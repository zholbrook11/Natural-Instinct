using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private Vector3 previousMousePosition;
    private Vector3 mouseDelta;

    public GameObject target; // Assign in the Inspector (the cube to rotate)

    private float speed = 200f;

    void Update()
    {
        Swipe();
        Drag();
    }

    void Drag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousMousePosition = Input.mousePosition; // Store position when right mouse button is first pressed
        }
        else if (Input.GetMouseButton(1))
        {
            mouseDelta = Input.mousePosition - previousMousePosition;
            previousMousePosition = Input.mousePosition;

            mouseDelta *= 0.1f; // Control rotation sensitivity

            // Rotate this object around its center
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0f) * transform.rotation;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // Smoothly rotate back to target rotation
            if (target != null && transform.rotation != target.transform.rotation)
            {
                float step = speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
            }
        }
    }

    void Swipe()
    {
        if (Input.GetMouseButtonDown(1))
        {
            firstPressPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            secondPressPos = Input.mousePosition;
            currentSwipe = secondPressPos - firstPressPos;

            if (currentSwipe.magnitude < 50f) return; // Ignore small accidental swipes

            currentSwipe.Normalize();

            if (target == null) return;

            if (LeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 90, 0, Space.World);
            }
            else if (RightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, -90, 0, Space.World);
            }
            else if (UpLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90, 0, 0, Space.World);
            }
            else if (UpRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, -90, Space.World);
            }
            else if (DownLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, 90, Space.World);
            }
            else if (DownRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90, 0, 0, Space.World);
            }
        }
    }

    bool LeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && Mathf.Abs(swipe.y) < 0.5f;
    }

    bool RightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && Mathf.Abs(swipe.y) < 0.5f;
    }

    bool UpLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x < 0;
    }

    bool UpRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x > 0;
    }

    bool DownLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x < 0;
    }

    bool DownRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x > 0;
    }
}