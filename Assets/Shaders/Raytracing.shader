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
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols
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

            half4 _CameraParams;
            int _Reflections;
            int _RaysPerPixel;
            StructuredBuffer<Triangle> _Triangles;
            StructuredBuffer<Sphere> _Spheres;
            StructuredBuffer<MeshData> _Meshes;
            int _SpheresCount;
            int _MeshesCount;
            int _RenderedFrames;
            sampler2D _PreviousFrame;
            
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

            bool intersectAABB(float3 rayOrigin, float3 rayDir, float3 boxMin, float3 boxMax)
            {
                const float3 tMin = (boxMin - rayOrigin) / rayDir;
                const float3 tMax = (boxMax - rayOrigin) / rayDir;
                float3 t1 = min(tMin, tMax);
                float3 t2 = max(tMin, tMax);
                const float tNear = max(max(t1.x, t1.y), t1.z);
                const float tFar = min(min(t2.x, t2.y), t2.z);

                return tNear <= tFar;
            }
            
            HitResult GetHitResult(const Ray ray)
            {
                HitResult result = CreateHitResult();

                for (int i = 0; i < _SpheresCount; ++i)
                {
                    const HitResult hit = HitSphere(ray, i);

                    if (hit.success && hit.distance < result.distance)
                    {
                        result = hit;
                    }
                }
                
                for (int meshIndex = 0; meshIndex < _MeshesCount; ++meshIndex)
                {
                    if (intersectAABB(ray.origin, ray.direction, _Meshes[meshIndex].boundsMin, _Meshes[meshIndex].boundsMax))
                    {
                        const int startIndex = _Meshes[meshIndex].startIndex;
                        const int endIndex = _Meshes[meshIndex].startIndex + _Meshes[meshIndex].trianglesCount;
                         
                        for (int i = startIndex; i < endIndex; ++i)
                        {
                            const Triangle tris = _Triangles[i];
                            const HitResult hit = HitTriangle(ray, tris.a, tris.b, tris.c);
                    
                            if (hit.success && hit.distance < result.distance)
                            {
                                result = hit;
                                result.material = _Meshes[meshIndex].material;
                            }
                        }
                    }
                }
                
                return result;
            }
            
            half4 TraceRay(Ray ray, uint seed)
            {
                half4 rayColor = 1;
                half4 emittedLight = 0;
                
                for (int i = 0; i < _Reflections; ++i)
                {
                    const HitResult hit = GetHitResult(ray);

                    if (hit.success)
                    {
                        const float3 randomDirection = GetRandomHemisphereDirection(hit.normal, seed);
                        const float3 reflectionDirection = normalize(reflect(ray.direction, hit.normal));
                        ray.direction = lerp(randomDirection, reflectionDirection, hit.material.roughness);
                        ray.origin = hit.hitPoint;

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
                
                for (int j = 0; j < _RaysPerPixel; ++j)
                {
                    Random(seed);
                    resultColor += TraceRay(ray, seed);
                }
                
                return fixed4(resultColor / _RaysPerPixel, 1);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                const Ray ray = GetInitialRay(i.uv);
                const uint seed = GetSeed(i.uv);
                const fixed4 color = Trace(ray, seed);

                const float weight = 1.0 / (_RenderedFrames + 1);
                return saturate(tex2D(_PreviousFrame, i.uv) * (1 - weight) + color * weight);
            }
            ENDHLSL
        }
    }
}
