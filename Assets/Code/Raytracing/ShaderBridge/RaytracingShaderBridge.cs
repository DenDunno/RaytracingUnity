using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RaytracingShaderBridge
{
    [SerializeField] private Camera _camera;
    [SerializeField] private List<Sphere> _spheres;
    [SerializeField] private List<RayTracedMesh> _rayTracedMeshes;
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

    public void BufferData()
    {
        TryInitialize();
        PassCameraParameters();
        PassSpheres();
        PassMeshes();
        PassRaytracingParameters();
    }

    private void TryInitialize()
    {
        _trianglesBuffer ??= new TriangleBuffer(1000, Triangle.GetSize(), _rayTracedMeshes);
        _meshesBuffer ??= new RayTracedMeshBuffer(100, RayTracedMeshData.GetSize(), _rayTracedMeshes);
        _sphereBuffer ??= new SphereBuffer(100, SphereData.GetSize(), _spheres);
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

    private void PassSpheres()
    {
        _sphereBuffer.Map(_spheres.Count);
        _material.SetBuffer(_indices.SpheresData, _sphereBuffer.Container);
        _material.SetInt(_indices.NumOfSpheres, _spheres.Count);
    }

    private void PassMeshes()
    {
        _rayTracedMeshes.ForEach(mesh => mesh.UpdateTriangles());
        
        _trianglesBuffer.Map(_rayTracedMeshes.Sum(mesh => mesh.TrianglesCount));
        _meshesBuffer.Map(_rayTracedMeshes.Count);
        
        _material.SetBuffer(_indices.Meshes, _meshesBuffer.Container);
        _material.SetBuffer(_indices.Triangles, _trianglesBuffer.Container);
        
        _material.SetInt(_indices.MeshesCount, _rayTracedMeshes.Count);
    }

    private void PassRaytracingParameters()
    {
        _material.SetInt(_indices.NumOfReflections, _numOfReflections);
        _material.SetInt(_indices.NumOfRays, _numOfRays);
    }
}