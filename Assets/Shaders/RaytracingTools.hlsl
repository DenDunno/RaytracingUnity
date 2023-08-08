#include "UnityCG.cginc"

struct Ray
{
    float3 origin;
    float3 direction;
};

struct RayTracingMaterial
{
    float4 color;
    float4 emissionColor;
    float emissionStrength;
    float roughness;
};

struct HitResult
{
    int success;
    float3 hitPoint;
    float distance;
    float3 normal;
    RayTracingMaterial material;
};

struct Sphere
{
    float radius;
    float3 position;
    RayTracingMaterial material;
};

HitResult CreateHitResult()
{
    HitResult result;
    result.success = 0;
    result.hitPoint = float3(0,0,0);
    result.normal = float3(0,0,0);
    result.distance = 1000000000;
    result.material.color = half4(0,0,0,1);
    result.material.emissionColor = half4(0,0,0,1);
    result.material.emissionStrength = 0;
    result.material.roughness = 0;
    
    return result;
}

Ray CreateRay(float3 origin, float3 direction)
{
    Ray ray;
    ray.direction = direction;
    ray.origin = origin;
    
    return ray;
}

struct Triangle
{
    float3 a;
    float3 b;
    float3 c;
};

struct MeshData
{
    uint startIndex;
    uint trianglesCount;
    float3 boundsMin;
    float3 boundsMax;
    RayTracingMaterial material;
};

uint GetSeed(float2 uv)
{
    uint2 pixel = _ScreenParams.xy * uv;
    
    return pixel.x + pixel.y * _ScreenParams.x;
}

HitResult HitTriangle(const Ray ray, float3 a, float3 b, float3 c)
{
    HitResult result = CreateHitResult();
    const float3 normal = normalize(cross(b - a, c - a));
    const float dotRayNormal = dot(ray.direction, normal);
    const float distance = dot(a - ray.origin, normal) / dotRayNormal;
    
    if (dotRayNormal < 0 && distance > 0)
    {
        result.normal = normal;
        result.distance = distance;
        result.hitPoint = result.distance * ray.direction + ray.origin;
        result.success = dot(result.hitPoint - a, cross(normal, b - a)) >= 0 &&
                         dot(result.hitPoint - b, cross(normal, c - b)) >= 0 &&
                         dot(result.hitPoint - c, cross(normal, a - c)) >= 0;
    }
    
    return result;
}

HitResult HitSphere(const Ray ray, const float3 position, const float radius)
{
    HitResult result = CreateHitResult();
                
    if (dot(position - ray.origin, ray.direction) < 0)
    {
        return result;
    }

    const float a = 1; // a = dot(direction, direction) = length(direction) = 1, direction is already normalized
    const float b = 2 * dot(ray.direction, ray.origin - position);
    const float c = dot(ray.origin - position, ray.origin - position) - radius * radius;
    const float discriminant = b * b - 4 * a * c;

    result.success = discriminant >= 0;
                 
    if (result.success)
    {
        const float x1 = (-b + sqrt(discriminant)) / (2 * a);
        const float x2 = (-b - sqrt(discriminant)) / (2 * a);
        const float distance = min(x1, x2);

        if (distance < result.distance)
        {
            result.hitPoint = ray.origin + distance * ray.direction;
            result.normal = normalize(result.hitPoint - position);
            result.distance = distance;
        }
    }

    return result;
}