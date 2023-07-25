using System;
using UnityEngine;

[Serializable]
public class RaytracingShaderBridge
{
    [SerializeField] private Camera _camera;
    private static readonly int _screenSize = Shader.PropertyToID("_CameraParams");

    public void TransferData(Material material)
    {
        Vector3 bottomLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        Vector3 bottomRightPoint = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.nearClipPlane));
        Vector3 upperLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 1, _camera.nearClipPlane));

        float planeHeight = (bottomLeftPoint - upperLeftPoint).magnitude;
        float planeWidth = (bottomLeftPoint - bottomRightPoint).magnitude;
        
        material.SetVector(_screenSize, new Vector4(planeWidth, planeHeight, _camera.nearClipPlane, 0));
    }
}