using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WalkBehindFaderSetup
{
    static WalkBehindFaderSetup()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    static void OnHierarchyChanged()
    {
        int sortingLayerID = SortingLayer.NameToID("WalkBehind");
        SpriteRenderer[] allRenderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

        foreach (SpriteRenderer sr in allRenderers)
        {
            if (sr.sortingLayerID != sortingLayerID) continue;
            if (sr.GetComponent<WalkBehindFader>() != null) continue;

            Undo.AddComponent<WalkBehindFader>(sr.gameObject);
        }
    }

    [MenuItem("Tools/Add WalkBehindFader to All WalkBehind Sprites")]
    static void AddFaders()
    {
        int sortingLayerID = SortingLayer.NameToID("WalkBehind");
        SpriteRenderer[] allRenderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

        int count = 0;
        foreach (SpriteRenderer sr in allRenderers)
        {
            if (sr.sortingLayerID != sortingLayerID) continue;
            if (sr.GetComponent<WalkBehindFader>() != null) continue;

            Undo.AddComponent<WalkBehindFader>(sr.gameObject);
            count++;
        }

        Debug.Log($"WalkBehindFader added to {count} objects.");
    }
}
