using UnityEngine;

[ExecuteAlways]
public class Raytracing : MonoBehaviour
{
    [SerializeField] private bool _useRaytracing;
    [SerializeField] private Material _material;
    [SerializeField] private RaytracingShaderBridge _bridge;
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_useRaytracing)
        {
            _bridge.TransferData(_material);
            Graphics.Blit(source, destination, _material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}