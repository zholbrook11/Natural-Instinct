using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;
    [Tooltip("Color for single-hand highlight")]
    public Material highlightMaterial;

    [Tooltip("Color for dual-hand highlight")]
    public Material dualHighlightMaterial;

    private class HighlightInfo
    {
        public Material originalMaterial;
        public int highlightCount = 0;
    }

    private Dictionary<Renderer, HighlightInfo> trackedObjects = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddHighlight(Renderer rend)
    {
        if (!rend) return;

        if (!trackedObjects.TryGetValue(rend, out var info))
        {
            info = new HighlightInfo
            {
                originalMaterial = rend.material,
                highlightCount = 1
            };
            trackedObjects[rend] = info;
            rend.material = highlightMaterial;
        }
        else
        {
            info.highlightCount++;
            if (info.highlightCount == 2)
            {
                rend.material = dualHighlightMaterial;
            }
        }
    }

    public void RemoveHighlight(Renderer rend)
    {
        if (!rend || !trackedObjects.TryGetValue(rend, out var info))
            return;

        info.highlightCount--;

        if (info.highlightCount <= 0)
        {
            rend.material = info.originalMaterial;
            trackedObjects.Remove(rend);
        }
        else if (info.highlightCount == 1)
        {
            rend.material = highlightMaterial;
        }
    }




}