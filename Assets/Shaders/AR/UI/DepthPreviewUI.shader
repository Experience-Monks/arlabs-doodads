Shader "Jam3/AR/UI/DepthPreviewUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _OcclusionOffsetMeters ("Occlusion offset [meters]", Float) = 0

        _RenderType ("RenderType", Float) = 0

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

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

            //Global
            float4x4 _DisplayRotationPerFrame;

            //Texture
            sampler2D _MainTex;
            float4 _MainTex_ST;

            //Variables
            float _OcclusionOffsetMeters;
            float _RenderType;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 depthUv = mul(float3(i.uv, 1.0f), _DisplayRotationPerFrame).xy;
                // float depthMeters = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_MainTex, depthUv)) - _OcclusionOffsetMeters;

                float4 finalColor = float4(1, 1, 1, 1);
                float4 depthTexture = tex2Dlod(_MainTex, float4(depthUv, 0, 0)).rgba;

                if(_RenderType < 0.5 )
                {
                    finalColor = float4(depthTexture.rgb, 1.0);
                }
                else if(_RenderType > 0.5 && _RenderType < 1.5)
                {
                    finalColor = float4(depthTexture.rrr, 1.0);
                }
                else
                {
                    finalColor = float4(depthTexture.aaa, 1.0);
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
