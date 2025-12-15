Shader "FlowOn/Soft4D/BubbleTesseractCropped"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0.494, 0.773, 0.839, 0.85)
        _Color("Sprite Color", Color) = (1, 1, 1, 1)
        _EmissionColor("Emission Color", Color) = (0.494, 0.773, 0.839, 1)
        _EmissionStrength("Emission Strength", Float) = 1.2
        _DisplaceAmp("Displace Amplitude", Float) = 0.08
        _BulgeFreq("Bulge Frequency", Float) = 1.3
        _Pulse("Pulse", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "CanUseSpriteAtlas" = "True"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Forward"

            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float4 _Color;
            float4 _EmissionColor;
            float _EmissionStrength;
            float _DisplaceAmp;
            float _BulgeFreq;
            float _Pulse;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float Hash(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898, 78.233, 37.719))) * 43758.5453);
            }

            float SimpleNoise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);

                float n000 = Hash(i + float3(0, 0, 0));
                float n001 = Hash(i + float3(0, 0, 1));
                float n010 = Hash(i + float3(0, 1, 0));
                float n011 = Hash(i + float3(0, 1, 1));
                float n100 = Hash(i + float3(1, 0, 0));
                float n101 = Hash(i + float3(1, 0, 1));
                float n110 = Hash(i + float3(1, 1, 0));
                float n111 = Hash(i + float3(1, 1, 1));

                float3 u = f * f * (3.0 - 2.0 * f);

                float n00 = lerp(n000, n100, u.x);
                float n01 = lerp(n001, n101, u.x);
                float n10 = lerp(n010, n110, u.x);
                float n11 = lerp(n011, n111, u.x);

                float n0 = lerp(n00, n10, u.y);
                float n1 = lerp(n01, n11, u.y);

                return lerp(n0, n1, u.z) * 2.0 - 1.0;
            }

            v2f Vert(appdata_t input)
            {
                UNITY_SETUP_INSTANCE_ID(input);

                v2f output;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionOS = input.vertex.xyz;
                float3 normalOS = input.normal;

                float time = _Time.y * 0.8;
                float noise = SimpleNoise(positionOS * _BulgeFreq + time);
                float bulge = (noise * 0.5 + 0.5) * _DisplaceAmp + _Pulse * 0.04;

                float3 displacedOS = positionOS + normalOS * bulge;
                float4 positionWS = mul(unity_ObjectToWorld, float4(displacedOS, 1.0));
                float3 normalWS = UnityObjectToWorldNormal(normalOS);

                output.pos = mul(UNITY_MATRIX_VP, positionWS);
                output.normalWS = normalWS;
                output.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                return output;
            }

            half4 Frag(v2f input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);

                float4 baseSample = tex2D(_MainTex, input.uv);
                float4 tinted = baseSample * _BaseColor * _Color;

                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 2.0);
                float fresnelWithBias = 0.1 + fresnel;
                float emissionStrength = _EmissionStrength * (1.0 + 0.35 * fresnelWithBias);

                float3 emission = _EmissionColor.rgb * emissionStrength;
                float3 color = tinted.rgb + emission * 0.18;

                return float4(color, saturate(tinted.a));
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
