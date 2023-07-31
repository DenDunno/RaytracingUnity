using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class RayVisualizer : MonoBehaviour
{
    [SerializeField] [Range(1, 100)] private int _xCount = 10;
    [SerializeField] [Range(1, 100)] private int _yCount = 10;
    [SerializeField] [Min(0)] private float _zOffset = 1;
    [SerializeField] private bool _showHitRays;
    private Vector3 _bottomLeftPoint;
    private Vector2 _rayOffset;
    private Camera _camera;

    private void OnValidate()
    {
        _camera ??= GetComponent<Camera>();
        
        _bottomLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        Vector3 bottomRightPoint = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.nearClipPlane));
        Vector3 upperLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 1, _camera.nearClipPlane));

        float planeHeight = (_bottomLeftPoint - upperLeftPoint).magnitude;
        float planeWidth = (_bottomLeftPoint - bottomRightPoint).magnitude;

        _rayOffset = new Vector2()
        {
            x = planeWidth / (_xCount + 1),
            y = planeHeight / (_yCount + 1),
        };
    }

    private void Update()
    {
        _bottomLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        
        for (int i = 1; i <= _yCount; ++i)
        {
            for (int j = 1; j <= _xCount; ++j)
            {
                Vector3 point = _bottomLeftPoint + transform.right * (_rayOffset.x * j) + transform.up * (i * _rayOffset.y);
                Shoot(point);
            }
        }
    }

    private void Shoot(Vector3 point)
    {
        Vector3 color = (point - transform.position).normalized;
        Ray ray = new()
        {
            origin = transform.position,
            direction = color
        };

        HitResult result = HitSphere(ray, new Vector3(2, 0, 1), 1);

        if (result.success && _showHitRays)
        {
            Debug.DrawLine(transform.position, result.hitPoint, Color.magenta);
        }
        else if (_showHitRays == false)
        {
            Debug.DrawLine(transform.position, point + (point - transform.position).normalized * _zOffset, new Color(color.x, color.y, color.z, 1));
        }
    }

    HitResult HitSphere(Ray ray, Vector3 centre, float radius)
    {
        HitResult result; 
        result.hitPoint = new Vector3(0,0,0);

        float a = Vector3.Dot(ray.direction, ray.direction); 
        float b = 2 * Vector3.Dot(ray.direction, ray.origin - centre);
        float c = Vector3.Dot(ray.origin - centre, ray.origin - centre) - radius * radius;
        float discriminant = b * b - 4 * a * c;

        result.success = discriminant >= 0;
                
        if (result.success)
        {
            float x1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float x2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

            float length = Mathf.Abs(x1) < Mathf.Abs(x2) ? x1 : x2;
            result.hitPoint = ray.origin + length * ray.direction;
        }
                
        return result;
    }
    
    struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;
    };

    struct HitResult
    {
        public bool success;
        public Vector3 hitPoint;
    };
}