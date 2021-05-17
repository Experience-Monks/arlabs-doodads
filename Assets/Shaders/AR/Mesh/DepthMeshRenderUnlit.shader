Shader "Jam3/AR/DepthMeshRender"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _CurrentDepthTexture("Current Depth Texture", 2D) = "" {}

        _NormalizedDepthMin ("Normalized Depth Min", Range(0,5)) = 0.3
        _NormalizedDepthMax ("Normalized Depth Max", Range(0,10)) = 4

        _TriangleConnectivityCutOff ("TriangleConnectivityCutOff", Range(0,100)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
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

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                uint id : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 customColor: TEXCOORD1;
                float normalizedDepth: TEXCOORD2;
                int clipValue: TEXCOORD3;
            };

            sampler2D _CurrentDepthTexture;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _TriangleConnectivityCutOff;
            float _NormalizedDepthMin;
            float _NormalizedDepthMax;
            fixed4 _Color;

            uniform float _FocalLengthX;
            uniform float _FocalLengthY;
            uniform float _PrincipalPointX;
            uniform float _PrincipalPointY;
            uniform int _ImageDimensionsX;
            uniform int _ImageDimensionsY;
            uniform float4x4 _VertexModelTransform;

            inline float ArCoreDepth_GetMeters(float2 uv)
            {
                // The depth texture uses TextureFormat.RGB565.
                float4 rawDepth = tex2Dlod(_CurrentDepthTexture, float4(uv, 0, 0));
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

            v2f vert (appdata v)
            {
                v2f o;

                o.normalizedDepth = 0;
                o.clipValue = 0;
                o.customColor = float4(0.0, 0.0, 0.0, 1.0);

                float2 texID = int3((uint)v.id % (uint)_ImageDimensionsX, (uint)v.id / (uint)_ImageDimensionsX, 0);

                float2 depthTexuv0 = float2(texID.x / (float)_ImageDimensionsX, texID.y / (float)_ImageDimensionsY);
                float2 depthTexuv1 = float2((texID.x + 1) / (float)_ImageDimensionsX, texID.y / (float)_ImageDimensionsY);
                float2 depthTexuv2 = float2(texID.x / (float)_ImageDimensionsX, texID.y / (float)(_ImageDimensionsY + 1));
                float2 depthTexuv3 = float2((texID.x + 1) / (float)_ImageDimensionsX, texID.y / (float)(_ImageDimensionsY + 1));

                depthTexuv0.xy = -depthTexuv0.yx;
                depthTexuv1.xy = -depthTexuv1.yx;
                depthTexuv2.xy = -depthTexuv2.yx;
                depthTexuv3.xy = -depthTexuv3.yx;

                o.uv = depthTexuv0;

                float4 depths;
                depths[0] = ArCoreDepth_GetMeters(depthTexuv0);
                if (depths[0] == 0)
                {
                    v.vertex = 0;
                    v.normal = 0;
                    o.clipValue = kClipOutValue;
                    o.customColor = float4(0, 0, 0, 1);
                    o.normalizedDepth = 0;
                    return o;
                }

                depths[1] = ArCoreDepth_GetMeters(depthTexuv1);
                depths[2] = ArCoreDepth_GetMeters(depthTexuv2);
                depths[3] = ArCoreDepth_GetMeters(depthTexuv3);

                float4 averageDepth = (depths[0] +
                depths[1] +
                depths[2] +
                depths[3]) * 0.25;
                float4 depthDev = abs(depths - averageDepth);
                float cutoff = _TriangleConnectivityCutOff;
                float4 branch_ = step(cutoff, depthDev);

                if (any(branch_))
                {
                    v.vertex = 0;
                    v.normal = 0;
                    o.clipValue = kClipOutValue;
                    o.customColor = float4(0, 0, 0, 1);
                    o.normalizedDepth = 0;
                }
                else
                {
                    v.vertex = GetVertex(texID.x, texID.y,  depths[0]);

                    float4 vertexRight = GetVertex(texID.x + 1, texID.y, depths[1]);
                    float4 vertexBottom = GetVertex(texID.x, texID.y + 1, depths[2]);

                    float3 sideBA = vertexRight - v.vertex;
                    float3 sideCA = vertexBottom - v.vertex;
                    float3 normal = normalize(cross(sideBA, sideCA));

                    v.normal = normal;
                    o.normal = normal;

                    o.clipValue = 0;

                    o.customColor = float4((v.normal + 1) * 0.5, 1);

                    float depthRange = _NormalizedDepthMax - _NormalizedDepthMin;
                    o.normalizedDepth = (depths[0] - _NormalizedDepthMin) / depthRange;
                }

                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                clip(i.clipValue);

                float4 finalColor = _Color; //i.normalizedDepth * 0.95;
                return finalColor;
            }
            ENDCG
        }
    }
}
