using System.Collections.Generic;
using UnityEngine;

public class RubiksCubeController : MonoBehaviour
{
    [Header("Cube Pieces")]
    public List<Transform> cubePieces; // Assign 8 cubelets in the Inspector

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private Vector3 previousMousePosition;
    private Vector3 mouseDelta;

    private bool isRotating = false;

    void Update()
    {
        if (!isRotating)
        {
            Swipe();
            Drag();
        }
    }

    void Drag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            mouseDelta = Input.mousePosition - previousMousePosition;
            mouseDelta *= 0.1f;
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
            previousMousePosition = Input.mousePosition;
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
            currentSwipe.Normalize();

            if (LeftSwipe(currentSwipe))
                StartCoroutine(RotateFace(Vector3.up, Vector3.right, 90));
            else if (RightSwipe(currentSwipe))
                StartCoroutine(RotateFace(Vector3.up, Vector3.right, -90));
            else if (UpLeftSwipe(currentSwipe))
                StartCoroutine(RotateFace(Vector3.right, Vector3.up, -90));
            else if (UpRightSwipe(currentSwipe))
                StartCoroutine(RotateFace(Vector3.forward, Vector3.up, -90));
            else if (DownLeftSwipe(currentSwipe))
                StartCoroutine(RotateFace(Vector3.forward, Vector3.up, 90));
            else if (DownRightSwipe(currentSwipe))
                StartCoroutine(RotateFace(Vector3.right, Vector3.up, 90));
        }
    }

    bool LeftSwipe(Vector2 swipe) => swipe.x < 0 && Mathf.Abs(swipe.y) < 0.5f;
    bool RightSwipe(Vector2 swipe) => swipe.x > 0 && Mathf.Abs(swipe.y) < 0.5f;
    bool UpLeftSwipe(Vector2 swipe) => swipe.y > 0 && swipe.x < 0;
    bool UpRightSwipe(Vector2 swipe) => swipe.y > 0 && swipe.x > 0;
    bool DownLeftSwipe(Vector2 swipe) => swipe.y < 0 && swipe.x < 0;
    bool DownRightSwipe(Vector2 swipe) => swipe.y < 0 && swipe.x > 0;

    IEnumerator<WaitForSeconds> RotateFace(Vector3 rotationAxis, Vector3 planeNormal, float angle)
    {
        isRotating = true;

        // Step 1: Find cubes on the plane
        List<Transform> rotatingCubes = new List<Transform>();
        foreach (Transform cube in cubePieces)
        {
            Vector3 localPos = transform.InverseTransformPoint(cube.position);
            float planeCoord = Vector3.Dot(localPos, planeNormal);
            if (Mathf.Abs(planeCoord - 0.5f) < 0.2f) // Adjust threshold as needed
            {
                rotatingCubes.Add(cube);
            }
        }

        // Step 2: Create pivot
        GameObject pivot = new GameObject("RotationPivot");
        pivot.transform.parent = this.transform;
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localRotation = Quaternion.identity;

        foreach (Transform cube in rotatingCubes)
        {
            cube.parent = pivot.transform;
        }

        // Step 3: Animate rotation
        Quaternion startRotation = pivot.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(angle, rotationAxis);

        float t = 0;
        float duration = 0.3f;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            pivot.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        // Step 4: Finish
        pivot.transform.rotation = endRotation;

        foreach (Transform cube in rotatingCubes)
        {
            cube.parent = this.transform;
        }

        Destroy(pivot);
        isRotating = false;
    }
}
