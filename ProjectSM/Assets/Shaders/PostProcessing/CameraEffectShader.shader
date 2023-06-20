Shader"Hidden/CameraEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Header(Jigger)] 
        [Space]_Sinx("Sin Input X",Range(0,3.14159265359)) = 0.0
        _Siny("Sin Input Y",Range(0,3.14159265359)) = 0.0
        _SinAmpx("Sin Amp X",Float) = 0.0
        _SinAmpy("Sin Amp Y",Float) = 0.0
        [Space(20)]
        [Header(GrayScale)]
        [ToggleOff] _GrayScale("Gray Scale", Float) = 0.0
        [Header(Zoom)]
        _ZoomScale("Zoom Scale", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "Camera Effect"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile __ _GRAYSCALE_OFF

            #include "UnityCG.cginc"

            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Sinx;
            float _Siny;
            float _SinAmpx;
            float _SinAmpy;
            float _ZoomScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x += _SinAmpx * sin(2.0 *_Sinx);
                uv.y += _SinAmpy * sin(2.0 *_Siny);
                // sample the texture
                half4 col = tex2D(_MainTex, uv * (1- _ZoomScale) + _ZoomScale * 0.5);
                #if !_GRAYSCALE_OFF
                    float grayscale = col.r * 0.299 + col.g * 0.587 + col.b * 0.114;
                    return grayscale;
                #endif
                return col;
            }
            ENDHLSL
        }
    }
}
