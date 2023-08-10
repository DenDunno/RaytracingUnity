using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RayTracedMesh : MonoBehaviour
{
    [SerializeField] private RayTracingMaterial _material;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    private readonly List<Triangle> _triangles = new();

    public int TrianglesCount => _meshFilter.sharedMesh.triangles.Length / 3;
    public IReadOnlyList<Triangle> Triangles => _triangles;

    private void OnValidate()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void UpdateTransform()
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
        return new RayTracedMeshData(startIndex, _triangles.Count, _meshRenderer.bounds.min, _meshRenderer.bounds.max, _material);
    }
    
    private Vector3 GetWorldPoint(Vector3 localVertex)
    {
        Vector4 localPoint = new(localVertex.x, localVertex.y, localVertex.z, 1);
        
        return transform.localToWorldMatrix * localPoint;
    }

    private void OnDrawGizmos()
    {
        Vector3 min = _meshRenderer.bounds.min;
        Vector3 max = _meshRenderer.bounds.max;

        DrawBoundingBox(min, max);
    }
    
    private void DrawBoundingBox(Vector3 min, Vector3 max)
    {
        Gizmos.color = Color.red;
        Vector3[] vertices = new Vector3[8];
        
        vertices[0] = new Vector3(min.x, min.y, min.z);
        vertices[1] = new Vector3(min.x, min.y, max.z);
        vertices[2] = new Vector3(max.x, min.y, max.z);
        vertices[3] = new Vector3(max.x, min.y, min.z);
        vertices[4] = new Vector3(min.x, max.y, min.z);
        vertices[5] = new Vector3(min.x, max.y, max.z);
        vertices[6] = new Vector3(max.x, max.y, max.z);
        vertices[7] = new Vector3(max.x, max.y, min.z);
        
        Gizmos.DrawLine(vertices[0], vertices[1]);
        Gizmos.DrawLine(vertices[1], vertices[2]);
        Gizmos.DrawLine(vertices[2], vertices[3]);
        Gizmos.DrawLine(vertices[3], vertices[0]);

        Gizmos.DrawLine(vertices[4], vertices[5]);
        Gizmos.DrawLine(vertices[5], vertices[6]);
        Gizmos.DrawLine(vertices[6], vertices[7]);
        Gizmos.DrawLine(vertices[7], vertices[4]);

        Gizmos.DrawLine(vertices[0], vertices[4]);
        Gizmos.DrawLine(vertices[1], vertices[5]);
        Gizmos.DrawLine(vertices[2], vertices[6]);
        Gizmos.DrawLine(vertices[3], vertices[7]);
    }
}