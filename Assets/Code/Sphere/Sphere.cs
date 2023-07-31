using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private RayTracingMaterial _material;
    [SerializeField] private float _radius;

    private void OnValidate()
    {
        transform.localScale = Vector3.one * _radius;
    }

    public SphereData GetData()
    {
        return new SphereData(_radius / 2, transform.position, _material);
    }
}