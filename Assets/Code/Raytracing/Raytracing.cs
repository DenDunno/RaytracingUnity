using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Raytracing : MonoBehaviour
{
    [SerializeField] private RaytracingShaderBridge _raytracingBridge;
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
            _raytracingBridge.DrawToTexture(_textures, _renderedFrames);
            CopyCurrentFrameToPrevious();
            DrawToScreen(destination);
            TryMoveSeed();
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void CopyCurrentFrameToPrevious()
    {
        Graphics.Blit(_textures.CurrentFrame, _textures.PreviousFrame);
    }

    private void DrawToScreen(RenderTexture destination)
    {
        Graphics.Blit(_textures.CurrentFrame, destination);
    }

    private void TryMoveSeed()
    {
        _renderedFrames += Application.isPlaying ? 1 : 0;
    }
}