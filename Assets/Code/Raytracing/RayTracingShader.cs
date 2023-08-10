using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class RayTracingShader
{
    [SerializeField] private ComputeShader _shader;
    
    public void Dispatch(int renderedFrames, RenderTexture currentFrame, RenderTexture previousFrame)
    {
        if (Application.isPlaying == false)
            return;
        
        _shader.SetTexture(0, "_CurrentFrame", currentFrame);
        _shader.SetTexture(0, "_PreviousFrame", previousFrame);
        _shader.SetInt("_RenderedFrames", renderedFrames);
        
        int threadX = Mathf.CeilToInt(Screen.width / 8f);
        int threadY = Mathf.CeilToInt(Screen.height / 8f);
        _shader.Dispatch(0, threadX, threadY, 1);
    }
}