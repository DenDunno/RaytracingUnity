using UnityEngine;

public class AccumulateTextures
{
    private RenderTexture _currentFrame;
    private RenderTexture _previousFrame;

    public RenderTexture CurrentFrame => _currentFrame;
    public RenderTexture PreviousFrame => _previousFrame;
    
    public void TryResize()
    {
        TryResizeTexture(ref _previousFrame);
        TryResizeTexture(ref _currentFrame);
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
}