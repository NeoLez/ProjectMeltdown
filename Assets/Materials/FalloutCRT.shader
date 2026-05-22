Shader "Custom/FalloutCRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.25
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.03

        // MÁS SUAVE
        _Curvature ("Screen Curvature", Range(0,0.05)) = 0.006

        _Glow ("Glow", Range(0,3)) = 1.25

        _GreenTint ("Green Tint", Color) = (0,1,0.7,1)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _ScanlineIntensity;
            float _NoiseIntensity;
            float _Curvature;
            float _Glow;
            float4 _GreenTint;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float rand(float2 co)
            {
                return frac(
                    sin(dot(co.xy, float2(12.9898,78.233)))
                    * 43758.5453
                );
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // CURVATURA SUAVE
                float2 centered = uv * 2.0 - 1.0;

                centered.x *= 1.0 + pow(abs(centered.y), 2.0) * _Curvature;
                centered.y *= 1.0 + pow(abs(centered.x), 2.0) * _Curvature;

                uv = centered * 0.5 + 0.5;

                // EVITA BORDES NEGROS
                uv = clamp(uv, 0.001, 0.999);

                fixed4 col = tex2D(_MainTex, uv);

                // GRAYSCALE
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // VERDE FALLOUT
                col.rgb = gray * _GreenTint.rgb * _Glow;

                // SCANLINES
                float scan =
                    sin(uv.y * 350.0) * 0.5 + 0.5;

                col.rgb *= lerp(
                    1.0,
                    scan,
                    _ScanlineIntensity
                );

                // NOISE
                float noise =
                    rand(uv + _Time.y * 0.1);

                col.rgb += noise * _NoiseIntensity;

                // VIGNETTE CASI INVISIBLE
                float vignette =
                    1.0 - smoothstep(
                        1.1,
                        1.6,
                        length(centered)
                    );

                col.rgb *= vignette;

                return col;
            }
            ENDCG
        }
    }
}