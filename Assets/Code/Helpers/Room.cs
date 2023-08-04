using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private List<RayTracedMesh> _rayTracedMesh;
    [SerializeField] private List<Sphere> _spheres;

    public List<RayTracedMesh> RayTracedMeshes => _rayTracedMesh;
    public List<Sphere> Spheres => _spheres;
}