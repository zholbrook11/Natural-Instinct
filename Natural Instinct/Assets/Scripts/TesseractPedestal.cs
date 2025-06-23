using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TesseractPedestal : MonoBehaviour
{
    public Transform snapPoint;
    public float snapDistance = 0.5f;
    public float snapSpeed = 5f;
    public float delayBeforeSceneChange = 1f;
    public string nextSceneName = "NextScene";
    public AudioClip snapSound;

    private bool isSnapping = false;
    private GameObject tesseract;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isSnapping || tesseract == null) return;

        float distance = Vector3.Distance(tesseract.transform.position, snapPoint.position);
        if (distance < snapDistance)
        {
            StartCoroutine(SnapAndChangeScene());
        }
    }

    private IEnumerator SnapAndChangeScene()
    {
        isSnapping = true;
        var rb = tesseract.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        float t = 0f;
        Vector3 startPos = tesseract.transform.position;
        Quaternion startRot = tesseract.transform.rotation;
        while (t < 1f)
        {
            t += Time.deltaTime * snapSpeed;
            tesseract.transform.position = Vector3.Lerp(startPos, snapPoint.position, t);
            tesseract.transform.rotation = Quaternion.Slerp(startRot, snapPoint.rotation, t);
            yield return null;
        }

        if (snapSound != null) audioSource.PlayOneShot(snapSound);

        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tesseract"))
        {
            tesseract = other.gameObject;
        }
    }
}
