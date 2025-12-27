Shader "UI/FlowOnFluid"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _PrimarySpeed ("Primary Speed", Vector) = (0.03, 0.02, 0, 0)
        _SecondarySpeed ("Secondary Speed", Vector) = (-0.02, 0.025, 0, 0)
        _PrimaryScale ("Primary Scale", Float) = 6
        _SecondaryScale ("Secondary Scale", Float) = 3.5
        _WarpStrength ("Warp Strength", Float) = 0.6
        _Brightness ("Brightness", Float) = 1
        _Alpha ("Alpha", Range(0, 1)) = 1
        _ColorA ("Color A", Color) = (0.108, 0.525, 1, 1)
        _ColorB ("Color B", Color) = (0.498, 0.192, 1, 1)
        _ColorC ("Color C", Color) = (1, 0.18, 0.651, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ClipRect;
            float4 _PrimarySpeed;
            float4 _SecondarySpeed;
            float _PrimaryScale;
            float _SecondaryScale;
            float _WarpStrength;
            float _Brightness;
            float _Alpha;
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 _ColorC;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                o.worldPosition = v.vertex;
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(5.3983, 5.4427));
                p += dot(p, p + 3.5453123);
                return frac((p.x + p.y) * p.y);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = hash21(i);
                float b = hash21(i + float2(1.0, 0.0));
                float c = hash21(i + float2(0.0, 1.0));
                float d = hash21(i + float2(1.0, 1.0));

                float lerpX1 = lerp(a, b, u.x);
                float lerpX2 = lerp(c, d, u.x);
                return lerp(lerpX1, lerpX2, u.y);
            }

            float fbm(float2 p, float baseScale)
            {
                float amplitude = 0.5;
                float sum = 0.0;
                float2 wave = p * baseScale;

                for (int i = 0; i < 4; i++)
                {
                    sum += noise(wave) * amplitude;
                    wave = wave * 2.03 + float2(17.0, 11.0);
                    amplitude *= 0.5;
                }

                return sum;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y;
                float2 uv = i.uv;

                float2 primaryOffset = float2(_PrimarySpeed.x, _PrimarySpeed.y) * time;
                float2 secondaryOffset = float2(_SecondarySpeed.x, _SecondarySpeed.y) * time;

                float primary = fbm(uv + primaryOffset, max(_PrimaryScale, 0.0001));
                float secondary = fbm(uv + secondaryOffset, max(_SecondaryScale, 0.0001));
                float warp = fbm(uv + float2(primary, secondary) * _WarpStrength + secondaryOffset * 0.5, max(_PrimaryScale * 0.8, 0.0001));

                float blend = saturate(primary * 0.55 + secondary * 0.25 + warp * 0.5);
                float3 baseColor = lerp(_ColorA.rgb, _ColorB.rgb, blend);
                baseColor = lerp(baseColor, _ColorC.rgb, saturate(warp));

                fixed4 texSample = tex2D(_MainTex, i.uv);
                fixed4 col = fixed4(baseColor * _Brightness, _Alpha);
                col *= texSample;
                col *= i.color;
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                col.rgb *= col.a;

                return col;
            }
            ENDCG
        }
    }
}
