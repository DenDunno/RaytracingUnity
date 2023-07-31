using System;
using UnityEngine;

[Serializable]
public struct RayTracingMaterial 
{
    [SerializeField] private Color _color;
    [SerializeField] private Color _emissionColor;
    [SerializeField] [Min(0)] private float _emissionStrength;
}