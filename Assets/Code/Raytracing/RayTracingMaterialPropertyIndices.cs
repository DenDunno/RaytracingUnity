using UnityEngine;

public class RayTracingMaterialPropertyIndices
{
    public readonly int SpheresData;
    public readonly int ScreenSize;
    public readonly int NumOfSpheres;
    public readonly int NumOfReflections;
    public readonly int NumOfRays;

    public RayTracingMaterialPropertyIndices()
    {
        SpheresData = Shader.PropertyToID("_Spheres");
        ScreenSize = Shader.PropertyToID("_CameraParams");
        NumOfSpheres = Shader.PropertyToID("_NumOfSpheres");
        NumOfReflections = Shader.PropertyToID("_NumOfReflections");
        NumOfRays = Shader.PropertyToID("_NumOfRays");
    }
}