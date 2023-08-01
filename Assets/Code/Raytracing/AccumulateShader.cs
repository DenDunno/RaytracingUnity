using System;
using UnityEngine;

[Serializable]
public class AccumulateShader
{
    [SerializeField] private ComputeShader _accumulateShader;

    public void Dispatch(int renderedFrames, RenderTexture currentFrame, RenderTexture previousFrame)
    {
        if (Application.isPlaying == false)
            return;
        
        _accumulateShader.SetTexture(0, "_CurrentFrame", currentFrame);
        _accumulateShader.SetTexture(0, "_PreviousFrame", previousFrame);
        _accumulateShader.SetInt("_RenderedFrames", renderedFrames);
        
        int threadX = Mathf.CeilToInt(Screen.width / 8f);
        int threadY = Mathf.CeilToInt(Screen.height / 8f);
        _accumulateShader.Dispatch(0, threadX, threadY, 1);
    }
}