using UnityEngine;

public struct RayTracedMeshData
{
    private readonly int _startIndex;
    private readonly int _trianglesCount;
    private readonly Vector3 _boundsMin;
    private readonly Vector3 _boundsMax;
    private readonly RayTracingMaterial _material;

    public RayTracedMeshData(int startIndex, int trianglesCount, Vector3 boundsMin, Vector3 boundsMax, RayTracingMaterial material)
    {
        _trianglesCount = trianglesCount;
        _startIndex = startIndex;
        _boundsMin = boundsMin;
        _boundsMax = boundsMax;
        _material = material;
    }
    
    public static int GetSize()
    {   
        return 4 + 4 + 12 + 12 + 36;
    }
}