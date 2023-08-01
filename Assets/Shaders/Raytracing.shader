Shader "Raytracing"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

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
                
                return result;
            }

            Ray CreateRay(float3 origin, float3 direction)
            {
                Ray ray;
                ray.direction = direction;
                ray.origin = origin;
                
                return ray;
            }

            half4 _CameraParams;
            int _NumOfReflections;
            int _NumOfRays;
            StructuredBuffer<Sphere> _Spheres;
            int _NumOfSpheres;
            int _RenderedFrames;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            HitResult HitSphere(const Ray ray, const int id)
            {
                HitResult result = CreateHitResult();
                
                if (dot(_Spheres[id].position - ray.origin, ray.direction) < 0)
                {
                    return result;
                }

                const float a = 1; // a = dot(direction, direction) = length(direction) = 1, direction is already normalized
                const float b = 2 * dot(ray.direction, ray.origin - _Spheres[id].position);
                const float c = dot(ray.origin - _Spheres[id].position, ray.origin - _Spheres[id].position) - _Spheres[id].radius * _Spheres[id].radius;
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
                        result.normal = normalize(result.hitPoint - _Spheres[id].position);
                        result.distance = distance;
                        result.material = _Spheres[id].material;
                    }
                }

                return result;
            }

            Ray GetInitialRay(float2 uv)
            {
                const float3 localViewPoint = float3(uv - 0.5, 1) * _CameraParams;
                const float3 worldViewPoint = mul(unity_CameraToWorld, float4(localViewPoint, 1));
                const float3 cameraPosition = _WorldSpaceCameraPos;

                return CreateRay(cameraPosition, normalize(worldViewPoint - cameraPosition));
            }

            uint GetSeed(float2 uv)
            {
                uint2 pixel = _ScreenParams.xy * uv;
                
                return pixel.x + pixel.y * _ScreenParams.x;
            }

            float Random(inout uint seed)
            {
                seed *= (seed + 195439) * (seed + 124395) * (seed + 845921) + _RenderedFrames * 123456;
                return seed / 4294967295.0;
            }
            
            float RandomNormalDistribution(inout uint seed)
            {
                const float3 randomValue1 = Random(seed);
                const float3 randomValue2 = Random(seed);
                
                return sqrt(-2 * log(randomValue1)) * 
                       cos(2 * UNITY_PI * randomValue2);
            }
            
            float3 GetRandomHemisphereDirection(const float3 normal, inout uint seed)
            {
                const float3 randomDirection = normalize(float3(
                    RandomNormalDistribution(seed),
                    RandomNormalDistribution(seed),
                    RandomNormalDistribution(seed)));
                
                return randomDirection * sign(dot(randomDirection, normal));
            }

            HitResult HitPlane(const Ray ray, float3 normal, float3 origin)
            {
                HitResult result = CreateHitResult();
                const float dotRayNormal = dot(ray.direction, normal);

                if (dotRayNormal >= 0)
                    return result;
                
                result.distance = dot(origin - ray.origin, normal) / dotRayNormal;
                result.normal = normal;
                result.success = result.distance >= 0;
                result.hitPoint = result.distance * ray.direction + ray.origin;
                result.material.color = float4(0,1,0,1);

                return result;
            }

            HitResult HitTriangle(const Ray ray, float3 a, float3 b, float3 c)
            {
                HitResult result = CreateHitResult();
                const float3 normal = normalize(cross(b - a, c - a));
                const float dotRayNormal = dot(ray.direction, normal);

                if (dotRayNormal < 0)
                {
                    result.normal = normal;
                    result.material.color = float4(0,1,0,0);
                    result.distance = dot(a - ray.origin, normal) / dotRayNormal;
                    result.hitPoint = result.distance * ray.direction + ray.origin;
                    result.success = dot(result.hitPoint - a, cross(normal, b - a)) >= 0 &&
                                     dot(result.hitPoint - b, cross(normal, c - b)) >= 0 &&
                                     dot(result.hitPoint - c, cross(normal, a - c)) >= 0;
                }
                
                return result;
            }
            
            HitResult GetHitResult(const Ray ray)
            {
                HitResult result = CreateHitResult();
                HitResult planeHit = HitTriangle(ray, _Spheres[0].position, _Spheres[1].position, _Spheres[2].position);

                if (planeHit.success)
                    result = planeHit;
                
                for (int i = 0; i < _NumOfSpheres; ++i)
                {
                    const HitResult hit = HitSphere(ray, i);

                    if (hit.success && hit.distance < result.distance)
                    {
                        result = hit;
                    }
                }

                return result;
            }
            
            half4 TraceRay(Ray ray, uint seed)
            {
                half4 rayColor = 1;
                half4 emittedLight = 0;
                
                for (int i = 0; i < _NumOfReflections; ++i)
                {
                    const HitResult hit = GetHitResult(ray);

                    if (hit.success)
                    {
                        ray.origin = hit.hitPoint;
                        ray.direction = GetRandomHemisphereDirection(hit.normal, seed);

                        const float light = max(dot(hit.normal, ray.direction), 0);
                        emittedLight += hit.material.emissionColor * hit.material.emissionStrength * rayColor;
                        rayColor *= hit.material.color * light * 2;
                    }
                    else
                    {
                        break;
                    }
                }
                
                return emittedLight;
            }

            fixed4 Trace(Ray ray, uint seed)
            {
                fixed3 resultColor;

                for (int j = 0; j < _NumOfRays; ++j)
                {
                    Random(seed);
                    resultColor += TraceRay(ray, seed);
                }
                
                return fixed4(resultColor / _NumOfRays, 1);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                const Ray ray = GetInitialRay(i.uv);
                const uint seed = GetSeed(i.uv);
                
                return Trace(ray, seed);
            }
            ENDCG
        }
    }
}
