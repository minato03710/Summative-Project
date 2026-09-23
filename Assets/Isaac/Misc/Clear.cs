#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RemovePolybrushComponents
{
    [MenuItem("Tools/Remove All Polybrush Components")]
    public static void RemoveAll()
    {
        int count = 0;
        // Find all GameObjects in the currently loaded scene
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            // Inspect all attached components
            Component[] components = go.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c != null && c.GetType().Name.Contains("Polybrush"))
                {
                    Undo.DestroyObjectImmediate(c);
                    count++;
                }
            }
        }

        // Mark scene as modified so Ctrl+S saves the changes
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"[Cleanup Success] Removed {count} Polybrush component(s) from the scene!");
    }
}
#endif