using UnityEngine;
using UnityEditor;

public class InvisibleWalling : MonoBehaviour
{
    [MenuItem("Tools/Add Invisible Walls to Hedges")]
    static void AddWalls()
    {
        string hedgeTag = "Hedge"; // Change this to match your hedge tag or name

        // Find all objects with the tag "Hedge"
        GameObject[] hedges = GameObject.FindGameObjectsWithTag(hedgeTag);

        if (hedges.Length == 0)
        {
            Debug.LogWarning("No hedges found with tag: " + hedgeTag);
            return;
        }

        foreach (GameObject hedge in hedges)
        {
            // Check if it already has an invisible wall
            if (hedge.transform.Find("InvisibleWall") != null)
                continue;

            // Create invisible collider as child
            GameObject wall = new GameObject("InvisibleWall");
            wall.transform.parent = hedge.transform;
            wall.transform.localPosition = Vector3.zero;
            wall.transform.localRotation = Quaternion.identity;
            wall.transform.localScale = Vector3.one;

            BoxCollider collider = wall.AddComponent<BoxCollider>();

            // Set a reasonable collider size
            collider.size = new Vector3(1f, 2f, 1f); // Customize as needed

            // Optionally: Assign to a special layer (e.g., "MazeCollision")
            wall.layer = LayerMask.NameToLayer("Default");

            Debug.Log("Added invisible wall to: " + hedge.name);
        }
    }
}