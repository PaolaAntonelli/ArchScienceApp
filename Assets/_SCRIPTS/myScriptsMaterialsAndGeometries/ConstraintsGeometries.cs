// Nota: Applica logiche simili a entrambi i file
using UnityEngine;

public class ConstraintsGeometries : MonoBehaviour
{
    public GameObject hingeObject;
    public GameObject rollerObject;
    public GameObject fixedObject;
    public Material constraintDefaultMaterial;
    public Material constraintActiveDefaultMaterial;

    public void NormalizeAndPivot(GameObject obj)
    {
        MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
        if (mf == null) return;

        // Centriamo l'oggetto affinché la "presa" XR sia naturale
        Bounds bounds = mf.sharedMesh.bounds;
        Vector3 offset = bounds.center;
        offset.z = bounds.max.z; // Allinea alla base della trave

        Vector3[] vertices = mf.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] -= offset;

        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateBounds();
    }
}