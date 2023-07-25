using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class ScreenPointsVisualizer : MonoBehaviour
{
    [SerializeField] [Range(1, 100)] private int _xCount = 10;
    [SerializeField] [Range(1, 100)] private int _yCount = 10;
    [SerializeField] [Min(0)] private float _zOffset = 1;
    [SerializeField] private bool _showHitRays = false;
    private Camera _camera;
    private Mesh _quadMesh;
    
    private void Update()
    {
        _quadMesh ??= MeshBuilder.CreateQuad(1, 1);
        _camera ??= GetComponent<Camera>();
        
        Vector3 bottomLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        Vector3 bottomRightPoint = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.nearClipPlane));
        Vector3 upperLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 1, _camera.nearClipPlane));

        float planeHeight = (bottomLeftPoint - upperLeftPoint).magnitude;
        float planeWidth = (bottomLeftPoint - bottomRightPoint).magnitude;
        
        float xOffset = planeWidth / (_xCount + 1);
        float yOffset = planeHeight / (_yCount + 1);
        
        for (int i = 1; i <= _yCount; ++i)
        {
            for (int j = 1; j <= _xCount; ++j)
            {
                Vector3 point = bottomLeftPoint + transform.right * (xOffset * j) + transform.up * (i * yOffset);
                Vector3 color = (point - transform.position).normalized;

                Ray ray = new Ray()
                {
                    origin = transform.position,
                    direction = color
                };

                HitResult result = HitSphere(ray, new Vector3(2, 0, 1), 1);

                if (result.success && _showHitRays)
                {
                    Debug.DrawLine(transform.position, point + (point - transform.position).normalized * _zOffset, Color.magenta);
                }
                else if (_showHitRays == false)
                {
                    Debug.DrawLine(transform.position, point + (point - transform.position).normalized * _zOffset, new Color(color.x, color.y, color.z, 1));
                }
            }
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
            result.hitPoint = length * ray.direction;
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