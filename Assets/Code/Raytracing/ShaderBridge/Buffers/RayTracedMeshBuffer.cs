using System.Collections.Generic;
using Unity.Collections;

public class RayTracedMeshBuffer : Buffer<RayTracedMeshData>
{
    private IReadOnlyList<RayTracedMesh> _meshes;

    public RayTracedMeshBuffer(int count, int stride) : base(count, stride)
    {
    }
    
    public void SetSource(IReadOnlyList<RayTracedMesh> meshes)
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