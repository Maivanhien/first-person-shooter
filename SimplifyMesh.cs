using UnityEngine;

public class SimplifyMesh : MonoBehaviour
{
    public float qualify = 0.5f;
    void Start()
    {
        var originalMesh = GetComponent<MeshFilter>().sharedMesh;
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(originalMesh);
        meshSimplifier.SimplifyMesh(qualify);
        var destMesh = meshSimplifier.ToMesh();
        GetComponent<MeshFilter>().sharedMesh = destMesh;
    }
}
