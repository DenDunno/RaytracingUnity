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

            half4 _CameraParams; 
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            struct Ray
            {
                float3 origin;
                float3 direction;
            };

            struct HitResult
            {
                int success;
                float3 hitPoint;
            };

            HitResult HitSphere(const Ray ray, const float3 centre, const float radius)
            {
                HitResult result;
                result.hitPoint = float3(0,0,0);
                
                if (dot(centre - ray.origin, ray.direction) < 0)
                {
                    result.success = 0;
                    return result;
                }

                const float a = dot(ray.direction, ray.direction); // a = dot(direction, direction) = length(direction) = 1, direction is already normalized
                const float b = 2 * dot(ray.direction, ray.origin - centre);
                const float c = dot(ray.origin - centre, ray.origin - centre) - radius * radius;
                const float discriminant = b * b - 4 * a * c;

                result.success = discriminant >= 0;
                
                if (result.success)
                {
                    const float x1 = (-b + sqrt(discriminant)) / (2 * a);
                    const float x2 = (-b - sqrt(discriminant)) / (2 * a);

                    const float length = abs(x1) < abs(x2) ? x1 : x2;
                    result.hitPoint = length * ray.direction;
                }
                
                return result;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                const float3 localViewPoint = float3(i.uv - 0.5, 1) * _CameraParams;
                const float3 worldViewPoint = mul(unity_CameraToWorld, float4(localViewPoint, 1));
                const float3 cameraPosition = _WorldSpaceCameraPos;

                Ray ray;
                ray.direction = normalize(worldViewPoint - cameraPosition);
                ray.origin = cameraPosition;

                const HitResult hitResult = HitSphere(ray, float3(2,0,1), 1);

                if (hitResult.success)
                    return fixed4(1,1,1,1);
                
                return fixed4(ray.direction, 1);
            }
            ENDCG
        }
    }
}
