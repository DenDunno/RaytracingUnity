using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Raytracing : MonoBehaviour
{
    [SerializeField] private RaytracingShaderBridge _raytracingBridge;
    [SerializeField] private AccumulateShader _accumulateShader;
    [SerializeField] private bool _useRaytracing;
    private readonly AccumulateTextures _textures = new();
    private int _renderedFrames;

    public void Enable(Room room)
    {
        _raytracingBridge.BufferData(room);
        _renderedFrames = 0;
        _useRaytracing = true;
    }

    public void Disable()
    {
        _useRaytracing = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_useRaytracing)
        {
            _textures.TryResize();
            _raytracingBridge.DrawToTexture(_textures.CurrentFrame, _renderedFrames);
            _accumulateShader.Dispatch(_renderedFrames, _textures.CurrentFrame, _textures.PreviousFrame);
            DrawToScreen(destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void DrawToScreen(RenderTexture destination)
    {
        Graphics.Blit(_textures.CurrentFrame, _textures.PreviousFrame);
        Graphics.Blit(_textures.CurrentFrame, destination);
        _renderedFrames += Application.isPlaying ? 1 : 0;
    }
}