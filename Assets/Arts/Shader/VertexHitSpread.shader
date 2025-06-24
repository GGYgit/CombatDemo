Shader "Custom/VertexHitSpread"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _HitColor ("Hit Color", Color) = (1,0,0,1)
        _HitPosition ("Hit Position (Local)", Vector) = (0,0,0,0)
        _HitRadius ("Hit Radius", Float) = 0
        _HitFalloff ("Hit Falloff", Range(0.1,2)) = 0.5
        _HitIntensity ("Hit Intensity", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 1
        _DissolveNoise ("Dissolve Noise", 2D) = "white" {}
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1,1,1,1)
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0.01,0.2)) = 0.05
        _DissolveRadius ("Dissolve Radius", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 localPos : TEXCOORD3; // 本地空间坐标
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _HitColor;
            float3 _HitPosition;
            float _HitRadius;
            float _HitFalloff;
            float _HitIntensity;
            float _Metallic;

            float _DissolveAmount;
            float _DissolveEdgeWidth;
            float4 _DissolveEdgeColor;
            sampler2D _DissolveNoise;
            float _DissolveRadius;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.localPos = v.vertex.xyz; // 保留为本地坐标
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float3 N = normalize(i.worldNormal);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(N, L));
                float3 diffuse = _LightColor0.rgb * NdotL;

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                float distHit = distance(i.localPos, _HitPosition);
                float hitFactor = saturate(1 - smoothstep(_HitRadius - _HitFalloff, _HitRadius, distHit));
                fixed4 hitColor = lerp(col, _HitColor, hitFactor * _HitIntensity);

                float3 baseColor = hitColor.rgb;

                if (_DissolveAmount >= 1) {
                    float3 finalColor = lerp(baseColor * diffuse, _LightColor0.rgb * baseColor, _Metallic);
                    return fixed4(finalColor, hitColor.a);
                }

                float baseNoise = tex2D(_DissolveNoise, i.uv2).r;
                float distDissolve = distance(i.localPos, _HitPosition);
                float distFactor = saturate(distDissolve * _DissolveRadius);
                float dissolveNoise = saturate(baseNoise + distFactor);

                float dissolve = smoothstep(_DissolveAmount - _DissolveEdgeWidth, _DissolveAmount + _DissolveEdgeWidth, dissolveNoise);
                float edge = smoothstep(_DissolveAmount, _DissolveAmount - _DissolveEdgeWidth, dissolveNoise);
                float alpha = 1.0 - dissolve;

                float3 finalColor = lerp(baseColor * diffuse, _LightColor0.rgb * baseColor, _Metallic);
                finalColor = lerp(finalColor, _DissolveEdgeColor.rgb, edge);

                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
