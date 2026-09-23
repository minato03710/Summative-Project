using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    void Combine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter finalMeshFilter = gameObject.AddComponent<MeshFilter>();
        finalMeshFilter.mesh = new Mesh();
        finalMeshFilter.mesh.CombineMeshes(combine);

        MeshRenderer finalRenderer = gameObject.AddComponent<MeshRenderer>();
        finalRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        gameObject.SetActive(true);
    }
}