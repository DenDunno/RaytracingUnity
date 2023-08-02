using System.Collections.Generic;
using Unity.Collections;

public class SphereBuffer : Buffer<SphereData>
{
    private readonly IReadOnlyList<Sphere> _spheres;

    public SphereBuffer(int count, int stride, IReadOnlyList<Sphere> spheres) : base(count, stride)
    {
        _spheres = spheres;
    }

    protected override void OnMap(ref NativeArray<SphereData> buffer)
    {
        for (int i = 0; i < buffer.Length; ++i)
        {
            buffer[i] = _spheres[i].GetData();
        }
    }
}