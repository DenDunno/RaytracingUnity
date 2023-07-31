using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Raytracing : MonoBehaviour
{
    [SerializeField] private RaytracingShaderBridge _bridge;
    [SerializeField] private ComputeShader _accumulateShader;
    [SerializeField] private bool _useRaytracing;
    [SerializeField] private bool _updateBuffering;

    private RenderTexture _currentFrame;
    private RenderTexture _previousFrame;
    private int _renderedFrames;

    private void Start()
    {
        _renderedFrames = 0;
    }
    
    [ContextMenu("Buffer data")]
    private void BufferData()
    {
        _bridge.BufferData();
    }

    private void Update()
    {
        if (_updateBuffering)
        {
            BufferData();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_useRaytracing)
        {
            TryResizeTexture(ref _previousFrame);
            TryResizeTexture(ref _currentFrame);
            
            Graphics.Blit(null, _currentFrame, _bridge.Material);
            DispatchAccumulateShader();
            Graphics.Blit(_currentFrame, _previousFrame);
            
            Graphics.Blit(_currentFrame, destination);
            
            _renderedFrames += Application.isPlaying ? 1 : 0;
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void TryResizeTexture(ref RenderTexture texture)
    {
        if (texture == null || Screen.width != texture.width || Screen.height != texture.height)
        {
            if (texture != null)
            {
                texture.Release();
            }

            texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
            texture.enableRandomWrite = true;
            texture.Create();
        }
    }

    private void DispatchAccumulateShader()
    {
        _accumulateShader.SetTexture(0, "_CurrentFrame", _currentFrame);
        _accumulateShader.SetTexture(0, "_PreviousFrame", _previousFrame);
        _accumulateShader.SetInt("_RenderedFrames", _renderedFrames);
        
        int threadX = Mathf.CeilToInt(Screen.width / 8f);
        int threadY = Mathf.CeilToInt(Screen.height / 8f);
        _accumulateShader.Dispatch(0, threadX, threadY, 1);
    }
}