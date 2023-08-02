using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RayTracedMesh : MonoBehaviour
{
    [SerializeField] private RayTracingMaterial _material;
    [SerializeField] private MeshFilter _meshFilter;
    private readonly List<Triangle> _triangles = new();

    public int TrianglesCount => _meshFilter.sharedMesh.triangles.Length / 3;
    public IReadOnlyList<Triangle> Triangles => _triangles;

    private void OnValidate()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void UpdateTriangles()
    {
        Vector3[] vertices = _meshFilter.sharedMesh.vertices;
        int[] triangles = _meshFilter.sharedMesh.triangles;
        _triangles.Clear();
        
        for (int i = 0; i < triangles.Length; i += 3)
        {
            _triangles.Add(new Triangle(
                GetWorldPoint(vertices[triangles[i + 0]]), 
                GetWorldPoint(vertices[triangles[i + 1]]), 
                GetWorldPoint(vertices[triangles[i + 2]])));
        }
    }

    public RayTracedMeshData GetData(int startIndex)
    {
        return new RayTracedMeshData(startIndex, _triangles.Count, Vector3.zero, Vector3.zero, _material);
    }

    private Vector3 GetWorldPoint(Vector3 localVertex)
    {
        Vector4 localPoint = new(localVertex.x, localVertex.y, localVertex.z, 1);
        
        return transform.localToWorldMatrix * localPoint;
    }
}