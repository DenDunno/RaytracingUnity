using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public readonly struct SphereData 
{
    private readonly float Radius;
    private readonly Vector3 Position;
    private readonly RayTracingMaterial Material;

    public SphereData(float radius, Vector3 position, RayTracingMaterial material)
    {
        Position = position;
        Material = material;
        Radius = radius;
    }

    public static int GetSize()
    {
        return 36 + 4 + 3 * 4;
    }
}