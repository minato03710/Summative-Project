#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class Clear
{
    [MenuItem("Tools/Purge Polybrush Scripts Only")]
    public static void PurgePolybrushComponents()
    {
        int componentCount = 0;
        int objectCount = 0;

        // Find all GameObjects in the active scene
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allObjects)
        {
            if (go == null) continue;

            // Inspect all components attached to this GameObject
            Component[] components = go.GetComponents<Component>();
            bool objectHadPolybrush = false;

            foreach (Component c in components)
            {
                // Check if component exists and its type contains "Polybrush"
                if (c != null && c.GetType().Name.Contains("Polybrush"))
                {
                    Undo.DestroyObjectImmediate(c);
                    componentCount++;
                    objectHadPolybrush = true;
                }
            }

            if (objectHadPolybrush)
            {
                objectCount++;
            }
        }

        // Mark scene as modified so Ctrl+S saves the stripped components
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"[Cleanup Success] Purged {componentCount} Polybrush script(s) from {objectCount} GameObject(s). All scene meshes and objects were preserved!");
    }
}
#endif