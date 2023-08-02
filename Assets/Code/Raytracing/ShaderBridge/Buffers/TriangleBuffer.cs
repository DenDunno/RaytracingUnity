using System.Collections.Generic;
using Unity.Collections;

public class TriangleBuffer : Buffer<Triangle>
{
    private readonly IReadOnlyList<RayTracedMesh> _meshes;

    public TriangleBuffer(int count, int stride, IReadOnlyList<RayTracedMesh> meshes) : base(count, stride)
    {
        _meshes = meshes;
    }

    protected override void OnMap(ref NativeArray<Triangle> buffer)
    {
        int index = 0;
        foreach (RayTracedMesh mesh in _meshes)
        {
            for (int j = 0; j < mesh.Triangles.Count; ++j)
            {
                buffer[index] = mesh.Triangles[j];
                index++;
            }
        }
    }
}