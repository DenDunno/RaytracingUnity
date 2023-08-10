using UnityEngine;

public class RayTracingMaterialPropertyIndices
{
    public readonly int SpheresData;
    public readonly int ScreenSize;
    public readonly int NumOfSpheres;
    public readonly int NumOfReflections;
    public readonly int NumOfRays;
    public readonly int RenderedFrames;
    public readonly int MeshesCount;
    public readonly int Triangles;
    public readonly int Meshes;
    public readonly int PreviousFrame;
    
    public RayTracingMaterialPropertyIndices()
    {
        SpheresData = Shader.PropertyToID("_Spheres");
        ScreenSize = Shader.PropertyToID("_CameraParams");
        NumOfSpheres = Shader.PropertyToID("_SpheresCount");
        NumOfReflections = Shader.PropertyToID("_Reflections");
        NumOfRays = Shader.PropertyToID("_RaysPerPixel");
        RenderedFrames = Shader.PropertyToID("_RenderedFrames");
        MeshesCount = Shader.PropertyToID("_MeshesCount");
        Triangles = Shader.PropertyToID("_Triangles");
        Meshes = Shader.PropertyToID("_Meshes");
        PreviousFrame = Shader.PropertyToID("_PreviousFrame");
    }
}