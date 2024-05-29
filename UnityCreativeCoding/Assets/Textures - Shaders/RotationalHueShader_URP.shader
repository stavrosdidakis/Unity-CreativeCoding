Shader "Custom/RotationalHueShader_URP"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _HueSpeed("Hue Speed", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float _HueSpeed;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            float3 RGBToHSV(float3 rgb)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = rgb.g < rgb.b ? float4(rgb.bg, K.wz) : float4(rgb.gb, K.xy);
                float4 q = rgb.r < p.x ? float4(p.xyw, rgb.r) : float4(rgb.r, p.yzx);

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSVToRGB(float3 hsv)
            {
                float3 K = float3(1.0, 2.0 / 3.0, 1.0 / 3.0);
                float3 P = abs(frac(hsv.xxx + K) * 6.0 - 3.0);
                return hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float3 hsv = RGBToHSV(col.rgb);
                hsv.x = frac(hsv.x + _Time.y * _HueSpeed);
                col.rgb = HSVToRGB(hsv);
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
