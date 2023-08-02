using System;
using UnityEngine;

[Serializable]
public struct RayTracingMaterial 
{
    [SerializeField] private Color _color;
    [SerializeField] private Color _emissionColor;
    [SerializeField] [Min(0)] private float _emissionStrength;
    [SerializeField] [Range(0, 1)] private float _roughness;

    public static int GetSize()
    {
        return 16 + 16 + 4 + 4;
    }
}