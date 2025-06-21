using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public Transform planeTransform;          // Assign this in the inspector
    public Vector3 shrinkToScale = new Vector3(0.1f, 0.1f, 0.1f); // Target shrink scale
    public float shrinkDuration = 1.0f;       // Duration to shrink

    private bool hasShrunk = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InRubiks")  && !hasShrunk)
        {
            StartCoroutine(ShrinkPlane());
            Destroy(other.gameObject); // Destroys the trigger after entering
            hasShrunk = true;
        }
    }

    private System.Collections.IEnumerator ShrinkPlane()
    {
        Vector3 originalScale = planeTransform.localScale;
        float timer = 0f;

        while (timer < shrinkDuration)
        {
            planeTransform.localScale = Vector3.Lerp(originalScale, shrinkToScale, timer / shrinkDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        planeTransform.localScale = shrinkToScale;
    }
}
