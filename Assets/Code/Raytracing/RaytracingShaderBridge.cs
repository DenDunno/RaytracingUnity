using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class RaytracingShaderBridge
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Sphere[] _spheres;
    [SerializeField] private Material _material;
    [SerializeField] [Min(1)] private int _numOfRays = 10;
    [SerializeField] [Min(1)] private int _numOfReflections = 1;
    private RayTracingMaterialPropertyIndices _indices = new();
    private ComputeBuffer _buffer;

    public void BufferData()
    {
        TryInitialize();
        PassCameraParameters();
        PassSpheres();
        PassRaytracingParameters();
    }

    public void DrawToTexture(RenderTexture currentFrame, int renderedFrames)
    {
        _material.SetInt(_indices.RenderedFrames, renderedFrames);
        Graphics.Blit(null, currentFrame, _material);
    }

    private void TryInitialize()
    {
        _buffer ??= new ComputeBuffer(100, SphereData.GetSize(), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
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
        NativeArray<SphereData> spheres = _buffer.BeginWrite<SphereData>(0, _spheres.Length);

        for (int i = 0; i < spheres.Length; ++i)
        {
            spheres[i] = _spheres[i].GetData();
        }

        _buffer.EndWrite<SphereData>(_spheres.Length);

        _material.SetBuffer(_indices.SpheresData, _buffer);
        _material.SetInt(_indices.NumOfSpheres, _spheres.Length);
    }

    private void PassRaytracingParameters()
    {
        _material.SetInt(_indices.NumOfReflections, _numOfReflections);
        _material.SetInt(_indices.NumOfRays, _numOfRays);
    }
}