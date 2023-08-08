using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class RaytracingShaderBridge
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Material _material;
    [SerializeField] [Min(1)] private int _numOfRays = 10;
    [SerializeField] [Min(1)] private int _numOfReflections = 1;
    private RayTracingMaterialPropertyIndices _indices = new();
    private SphereBuffer _sphereBuffer;
    private RayTracedMeshBuffer _meshesBuffer;
    private TriangleBuffer _trianglesBuffer;

    public void DrawToTexture(RenderTexture currentFrame, int renderedFrames)
    {
        _material.SetInt(_indices.RenderedFrames, renderedFrames);
        Graphics.Blit(null, currentFrame, _material);
    }

    public void BufferData(Room room)
    {
        TryInitialize(room);
        PassCameraParameters();
        PassSpheres(room);
        PassMeshes(room);
        PassRaytracingParameters();
    }

    private void TryInitialize(Room room)
    {
        _trianglesBuffer ??= new TriangleBuffer(1000, Triangle.GetSize());
        _meshesBuffer ??= new RayTracedMeshBuffer(100, RayTracedMeshData.GetSize());
        _sphereBuffer ??= new SphereBuffer(100, SphereData.GetSize());
        
        _trianglesBuffer.SetSource(room.RayTracedMeshes);
        _meshesBuffer.SetSource(room.RayTracedMeshes);
        _sphereBuffer.SetSource(room.Spheres);
    }

    private void PassCameraParameters()
    {
        bool isSceneCam = Camera.current?.name == "SceneCamera";
        Camera camera = isSceneCam ? Camera.current : _camera;
        
        Vector3 bottomLeftPoint = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 bottomRightPoint = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));
        Vector3 upperLeftPoint = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));

        float planeHeight = (bottomLeftPoint - upperLeftPoint).magnitude;
        float planeWidth = (bottomLeftPoint - bottomRightPoint).magnitude;
        
        _material.SetVector(_indices.ScreenSize, new Vector4(planeWidth, planeHeight, camera.nearClipPlane, 0));
    }

    private void PassSpheres(Room room)
    {
        _sphereBuffer.Map(room.Spheres.Count);
        _material.SetBuffer(_indices.SpheresData, _sphereBuffer.Container);
        _material.SetInt(_indices.NumOfSpheres, room.Spheres.Count);
    }

    private void PassMeshes(Room room)
    {
        room.RayTracedMeshes.ForEach(mesh => mesh.UpdateTransform());
        
        _trianglesBuffer.Map(room.RayTracedMeshes.Sum(mesh => mesh.TrianglesCount));
        _meshesBuffer.Map(room.RayTracedMeshes.Count);
        
        _material.SetBuffer(_indices.Meshes, _meshesBuffer.Container);
        _material.SetBuffer(_indices.Triangles, _trianglesBuffer.Container);
        
        _material.SetInt(_indices.MeshesCount, room.RayTracedMeshes.Count);
    }

    private void PassRaytracingParameters()
    {
        _material.SetInt(_indices.NumOfReflections, _numOfReflections);
        _material.SetInt(_indices.NumOfRays, _numOfRays);
    }
}