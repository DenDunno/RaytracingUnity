using System.Collections.Generic;
using Unity.Collections;

public class SphereBuffer : Buffer<SphereData>
{
    private IReadOnlyList<Sphere> _spheres;

    public SphereBuffer(int count, int stride) : base(count, stride)
    {
    }
    
    public void SetSource(IReadOnlyList<Sphere> spheres)
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