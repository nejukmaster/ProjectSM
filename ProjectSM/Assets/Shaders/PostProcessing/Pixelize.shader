Shader"Hidden/Pixelize"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color",Color) = (0.0,0.0,0.0,1.0)

        _NormalMult("Normal Outline Multiplier", Range(0,4)) = 1
        _NormalBias("Normal Outline Bias", Range(1,10)) = 1
        _DepthMult("Depth Outline Multiplier", Range(0,100)) = 1
        _DepthBias("Depth Outline Bias", Range(1,10)) = 1

        _PixelInterpolation("Pixel Interpolation",Int) = 5

       [Toggle(GAUSS)] _Gauss ("Gaussian Blur", float) = 0
        _StandardDeviation("Standard Deviation (Gauss only)", Range(0, 0.01)) = 0.001

        [Toggle(OUTLINE)] _OutlineRender("Rendering Outline", Float) = 0
    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #pragma shader_feature OUTLINE
        #pragma shader_feature GAUSS

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

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

        //텍스쳐 오브젝트를 로드합니다.
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        float _NormalMult;
        float _NormalBias;
        float _DepthMult;
        float _DepthBias;
        int _PixelInterpolation;
        float _StandardDeviation;

        half4 _OutlineColor;

        //샘플러 선언
        SamplerState sampler_point_clamp;

        uniform float2 _BlockCount;
        uniform float2 _BlockSize;
        uniform float2 _HalfBlockSize;


        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            return OUT;
        }

        ENDHLSL

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM

            #define PI 3.14159265359
            #define E 2.71828182846

            void Compare(inout float depthOutline, inout float normalOutline,float2 uv) {
                //float3 neighborNormal = SampleSceneNormals(uv + _BlockSize.xy * offset);
                //float neighborDepth = SampleSceneDepth(uv + _BlockSize.xy * offset);

                float3x3 verticalOutlineConv = {1,0,-1,
                                                2,0,-2,
                                                1,0,-1};
                float3x3 horizontalOutlineConv = {1,2,1,
                                                0,0,0,
                                                -1,-2,-1};

                float depthDifferency_vert = 0;

                for(int i = 0; i < 9; i ++){
                    int x = i/3;
                    int y = i%3;

                    depthDifferency_vert += verticalOutlineConv[x][y] * SampleSceneDepth(uv + _BlockSize.xy * float2(x-2,y-2));
                }

                depthDifferency_vert = abs(depthDifferency_vert);

                float depthDifferency_horizon = 0;

                for(int i = 0; i < 9; i ++){
                    int x = i/3;
                    int y = i%3;

                    depthDifferency_horizon += horizontalOutlineConv[x][y] * SampleSceneDepth(uv + _BlockSize.xy * float2(x-2,y-2));
                }

                depthDifferency_horizon = abs(depthDifferency_horizon);

                //float3 normalDifference = baseNormal - neighborNormal;
                //normalDifference = normalDifference.r + normalDifference.g + normalDifference.b;
                //normalOutline = normalOutline + normalDifference;

                depthOutline = depthDifferency_horizon + depthDifferency_vert / 2;
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 blockPos = floor(IN.uv * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                half4 tex = 0.0;

                #if GAUSS
                if(_StandardDeviation == 0)
                        tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, blockCenter);
                else{
                        float sum = 0;
                        for (int i = 0; i < _BlockSize.x / _MainTex_TexelSize.x; i++) {
                            for (int j = 0; j < _BlockSize.y / _MainTex_TexelSize.y; j++) {
                                float offset = length(blockPos * _BlockSize + _MainTex_TexelSize * float2(i, j) - blockCenter);
                                float stDevSquared = _StandardDeviation * _StandardDeviation;
                                float gauss = (1 / sqrt(2 * PI * stDevSquared)) * pow(E, -((offset * offset) / (2 * stDevSquared)));
                                sum += gauss;
                                tex += SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, blockPos * _BlockSize + _MainTex_TexelSize * float2(i, j)) * gauss;
                            }
                        }
                        tex = tex/sum;
                }
                #else
                int sum = 0;
                for (int i = 0; i < _BlockSize.x / _MainTex_TexelSize.x; i++) {
                        for (int j = 0; j < _BlockSize.y / _MainTex_TexelSize.y; j++) {
                        tex = tex + SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, blockPos * _BlockSize + _MainTex_TexelSize * float2(i, j));
                        sum++;
                        }
                }
                tex = tex/sum;
                #endif

                //로딩한 텍스쳐를 샘플링합니다.
                //SAMPLE_TEXTURE2D(텍스쳐 오브젝트, 샘플러, uv)
                //float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, blockCenter);
                //return float4(IN.uv,1,1);
                
                #if OUTLINE
                    float3 normal = SampleSceneNormals(blockCenter);
                    float depth = SampleSceneDepth(blockCenter);
                    float normalDifference = 0;
                    float depthDifference = 0;

                    Compare(depthDifference, normalDifference, blockCenter);

                    normalDifference = normalDifference * _NormalMult;
                    normalDifference = saturate(normalDifference);
                    normalDifference = pow(normalDifference, _NormalBias);

                    depthDifference = depthDifference * _DepthMult;
                    depthDifference = saturate(depthDifference);
                    depthDifference = pow(depthDifference, _DepthBias);

                    float outline = (normalDifference + depthDifference);

                    float4 color = lerp(tex, _OutlineColor, outline);
                    return color;
                #endif
                return tex;
            }
            ENDHLSL
        }
    }
}