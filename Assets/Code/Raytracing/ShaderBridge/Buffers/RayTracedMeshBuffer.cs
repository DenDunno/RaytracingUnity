using System.Collections.Generic;
using Unity.Collections;

public class RayTracedMeshBuffer : Buffer<RayTracedMeshData>
{
    private readonly IReadOnlyList<RayTracedMesh> _meshes;

    public RayTracedMeshBuffer(int count, int stride, IReadOnlyList<RayTracedMesh> meshes) : base(count, stride)
    {
        _meshes = meshes;
    }

    protected override void OnMap(ref NativeArray<RayTracedMeshData> buffer)
    {
        int startIndex = 0;
        for (int i = 0; i < buffer.Length; ++i)
        {
            buffer[i] = _meshes[i].GetData(startIndex);
            startIndex += _meshes[i].TrianglesCount;
        }
    }
}