Shader "Jam3/AR/DepthMeshRender"
{
    Properties
    {
        _Color ("Color", Color) = (0.8,0,0,1)
        _ColorReady ("Color Ready", Color) = (0.1,0.9,0,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _RaWDepthTexture("Raw Depth Texture", 2D) = "black" {}
        _DepthTexture("Depth Texture", 2D) = "black" {}
        _ConfidenceTexture("Confidence Texture", 2D) = "black" {}

        _NormalizedDepthMin ("Normalized Depth Min", Range(0,5)) = 0.3
        _NormalizedDepthMax ("Normalized Depth Max", Range(0,10)) = 4
        _TriangleConnectivityCutOff ("TriangleConnectivityCutOff", Range(0,100)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "ForceNoShadowCasting" = "True"
        }
        Cull Off
        ZTest Always
        ZWrite On
        Lighting Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            static const float kClipOutValue = -10000000;

            #define ARCORE_DEPTH_SCALE 0.001        // mm to m
            #define ARCORE_MAX_DEPTH_MM 8191.0
            #define ARCORE_FLOAT_TO_5BITS 31        // (0.0, 1.0) -> (0, 31)
            #define ARCORE_FLOAT_TO_6BITS 63        // (0.0, 1.0) -> (0, 63)
            #define ARCORE_RGB565_RED_SHIFT 2048    // left shift 11 bits
            #define ARCORE_RGB565_GREEN_SHIFT 32    // left shift 5 bits
            #define ARCORE_BLEND_FADE_RANGE 0.01
            #define _PI 3.141516
            #define _PI2 6.28318530718

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 depthuv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            //Globals
            float4x4 _DisplayRotationPerFrame;

            //Textures
            sampler2D _RawDepthTexture;
            sampler2D _DepthTexture;
            sampler2D _ConfidenceTexture;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            //Variables
            float _TriangleConnectivityCutOff;
            float _NormalizedDepthMin;
            float _NormalizedDepthMax;
            float4 _Color;
            float4 _ColorReady;


            //Uniforms
            uniform float _isOver;
            uniform float _Amount;

            uniform sampler2D _EffectTexture;
            uniform float _FocalLengthX;
            uniform float _FocalLengthY;
            uniform float _PrincipalPointX;
            uniform float _PrincipalPointY;
            uniform int _ImageDimensionsX;
            uniform int _ImageDimensionsY;

            uniform float4x4 _VertexModelTransform;

            float4 Blur(sampler2D tex, float2 texUV, float blurSize, float steps, float quality, float downsampling)
            {
                float4 color = tex2D(tex, texUV);

                if(blurSize > 0.0)
                {
                    float2 radius = blurSize / _ScreenParams.yy;
                    for( float d = 0.0; d < _PI2; d += _PI2 / steps)
                    {
                        for(float i= 1.0 / quality; i <= 1.0; i += 1.0 / quality)
                        {
                            color += tex2Dlod(tex, float4(texUV + float2(cos(d), sin(d)) * radius * i, 0.0, downsampling));
                        }
                    }
                    color /= quality * steps;
                }

                return color;
            }

            inline float ArCoreDepth_GetMeters(float2 uv)
            {
                float4 rawDepth = tex2Dlod(_RawDepthTexture, float4(uv, 0, 0));
                float depth = (rawDepth.r * ARCORE_FLOAT_TO_5BITS * ARCORE_RGB565_RED_SHIFT)
                + (rawDepth.g * ARCORE_FLOAT_TO_6BITS * ARCORE_RGB565_GREEN_SHIFT)
                + (rawDepth.b * ARCORE_FLOAT_TO_5BITS);
                depth = min(depth, ARCORE_MAX_DEPTH_MM);
                depth *= ARCORE_DEPTH_SCALE;

                return depth;
            }

            float4 GetVertex(float tex_x, float tex_y, float z)
            {
                float4 vertex = 0;

                if (z > 0)
                {
                    float x = (tex_x - _PrincipalPointX) * z / _FocalLengthX;
                    float y = (tex_y - _PrincipalPointY) * z / _FocalLengthY;
                    vertex = float4(x, -y, z, 1);
                }

                vertex = mul(_VertexModelTransform, vertex);
                return vertex;
            }

            float3 HSVtoRGB(float3 color)
            {
                float4 K = float4(1.0h, 2.0h / 3.0h, 1.0h / 3.0h, 3.0h);
                float3 P = abs(frac(color.xxx + K.xyz) * 6.0h - K.www);
                return color.z * lerp(K.xxx, saturate(P - K.xxx), color.y);
            }

            v2f vert (appdata v, uint vertexID : SV_VertexID)
            {
                v2f o;
                o.uv = v.uv;

                float2 depthUv = mul(float3(v.uv, 1.0f), _DisplayRotationPerFrame).xy;
                o.depthuv = depthUv;
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                float depth = tex2D(_DepthTexture, i.depthuv).x;

                float blurAmount = 0;
                float confidence = Blur(_ConfidenceTexture, i.depthuv, blurAmount, 8, 1, 1).a;
                confidence = saturate(confidence);
                confidence = smoothstep(0.0, 1.0, confidence);

                float _MinDistance = 0;
                float _MaxDistance = 8;
                float lerpFactor = (depth - _MinDistance) / (_MaxDistance - _MinDistance);

                float4 gray = float4(0.5, 0.5, 0.5, 1.0);
                float4 gradColor = lerp(lerp(_Color, _ColorReady, _Amount), gray, _isOver) + (lerpFactor * 0.9);

                float rawDepth = ArCoreDepth_GetMeters(i.depthuv);

                // Use full ImageDimensions to get the real world position UV
                //float3 pos = GetVertex(i.depthuv.x * _ImageDimensionsX, i.depthuv.y * _ImageDimensionsY, rawDepth);

                float3 pos = GetVertex(i.depthuv.x * 10, i.depthuv.y * 20, rawDepth);
                fixed4 effectTexture = tex2D(_EffectTexture, pos.xz * 10) * confidence * 0.3 * gradColor;

                return lerp(color, lerp(color, gradColor, 0.35), confidence) + effectTexture;
            }
            ENDCG
        }
    }
}
