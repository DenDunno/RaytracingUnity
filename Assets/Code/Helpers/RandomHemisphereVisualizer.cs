using UnityEngine;

[ExecuteAlways]
public class RandomHemisphereVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject _plane;
    [SerializeField] private GameObject _sphere;
    [SerializeField] [Min(0)] private int _points;
    
    private void Update()
    {
        float radius = _sphere.transform.localScale.x / 2;
        Vector3 normal = _plane.transform.up;

        DrawRandomPoints(radius, normal, _points);
    }

    float RandomNormalDistribution(uint state)
    {
        return Mathf.Sqrt(-2 * Mathf.Log(Random.Range(0f, 1))) * 
               Mathf.Cos(2 * Mathf.PI * Random.Range(0f, 1));
    }
    
    Vector3 GetRandomSphereDirection()
    {
        return new Vector3()
        {
            x = RandomNormalDistribution(0),
            y = RandomNormalDistribution(0),
            z = RandomNormalDistribution(0),
        }.normalized;
    }

    private void DrawRandomPoints(float radius, Vector3 normal, int points)
    {
        for (int i = 0; i < points; ++i)
        {
            Vector3 randomDirection = GetRandomSphereDirection() * radius;
            randomDirection *= Mathf.Sign(Vector3.Dot(randomDirection, normal));
            Debug.DrawLine(_sphere.transform.position, _sphere.transform.position + randomDirection, Color.green);
        }
    }
}