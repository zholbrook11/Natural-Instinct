using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    [Header("Sound Categories")]
    public List<AudioSource> musicSources = new List<AudioSource>();
    public List<AudioSource> actionSources = new List<AudioSource>();
    public List<AudioSource> environmentSources = new List<AudioSource>();
    public List<AudioSource> playerSources = new List<AudioSource>();
    public List<AudioSource> animalSources = new List<AudioSource>();

    [Header("VR Settings UI")]
    public GameObject settingsCanvas;
  
    public Transform vrCamera;
    public Vector3 offsetFromHead = new Vector3(0f, -0.3f, 1.5f);
    public InputActionReference toggleMenuButton;

    private bool settingsVisible = false;

    [Header("Key References")]
    public Canvas keypadCanvas;
    public Transform player; // Reference to XR Rig or main camera

    [Header("Key Settings")]
    public float activationDistance = 2.5f;

    private CanvasGroup canvasGroup;
    private bool isPlayerNear = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, keypadCanvas.transform.position);

        if (distance <= activationDistance && !isPlayerNear)
        {
            isPlayerNear = true;
            SetCanvasState(true);
        }
        else if (distance > activationDistance && isPlayerNear)
        {
            isPlayerNear = false;
            SetCanvasState(false);
        }
    }

    private void Awake()
    {
        LoadSettings();
        toggleMenuButton.action.performed += ctx => ToggleSettings();
    }

    private void OnEnable()
    {
        toggleMenuButton?.action?.Enable();
    }

    private void OnDisable()
    {
        toggleMenuButton?.action?.Disable();
    }

    private void OnDestroy()
    {
        toggleMenuButton.action.performed -= ctx => ToggleSettings();
    }

    void SetCanvasState(bool state)
    {
        canvasGroup.alpha = state ? 1 : 0;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    private void ToggleSettings()
    {
        settingsVisible = !settingsVisible;
        settingsCanvas.SetActive(settingsVisible);
        //keypadCanvas.SetActive(settingsVisible);

        if (settingsVisible && vrCamera != null)
        {
            settingsCanvas.transform.position = vrCamera.position
                                                + vrCamera.forward * offsetFromHead.z
                                                + vrCamera.up * offsetFromHead.y;
            settingsCanvas.transform.LookAt(vrCamera);
            settingsCanvas.transform.Rotate(0, 180, 0); // flip to face the player
        }
    }

    public void SetCategoryEnabled(string category, bool enabled)
    {
        List<AudioSource> sources = GetCategoryList(category);
        foreach (var src in sources)
        {
            if (src != null) src.mute = !enabled;
        }
        PlayerPrefs.SetInt($"sound_{category}", enabled ? 1 : 0);
    }

    public void LoadSettings()
    {
        foreach (string category in new[] { "music", "action", "environment", "player", "animal" })
        {
            bool enabled = PlayerPrefs.GetInt($"sound_{category}", 1) == 1;
            SetCategoryEnabled(category, enabled);
        }
    }

    private List<AudioSource> GetCategoryList(string category)
    {
        return category switch
        {
            "music" => musicSources,
            "action" => actionSources,
            "environment" => environmentSources,
            "player" => playerSources,
            "animal" => animalSources,
            _ => new List<AudioSource>()
        };
    }

    private void Start()
    {
        musicSources.AddRange(FindAudioSourcesWithTag("MusicSound", musicSources));
        actionSources.AddRange(FindAudioSourcesWithTag("ActionSound", actionSources));
        environmentSources.AddRange(FindAudioSourcesWithTag("EnvironmentSound", environmentSources));
        playerSources.AddRange(FindAudioSourcesWithTag("PlayerSound", playerSources));
        animalSources.AddRange(FindAudioSourcesWithTag("AnimalSound", animalSources));

        if (keypadCanvas == null)
        {
            Debug.LogError("Settings Canvas not assigned.");
            return;
        }

        canvasGroup = keypadCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = keypadCanvas.gameObject.AddComponent<CanvasGroup>();
        }

        SetCanvasState(false);
    }


    private IEnumerable<AudioSource> FindAudioSourcesWithTag(string tag, List<AudioSource> currentList)
    {
        return GameObject.FindGameObjectsWithTag(tag)
            .Select(go => go.GetComponent<AudioSource>())
            .Where(src => src != null && !currentList.Contains(src));
    }
}
