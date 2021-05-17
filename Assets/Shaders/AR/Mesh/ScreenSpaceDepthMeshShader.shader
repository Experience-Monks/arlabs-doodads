//-----------------------------------------------------------------------
// <copyright file="ScreenSpaceDepthMeshShader.shader" company="Google LLC">
//
// Copyright 2020 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

Shader "Jam3/AR/Screen Space Depth Mesh Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _CurrentDepthTexture("Current Depth Texture", 2D) = "" {}

        _NormalizedDepthMin ("Normalized Depth Min", Range(0,5)) = 0.3
        _NormalizedDepthMax ("Normalized Depth Max", Range(0,10)) = 4

        _TriangleConnectivityCutOff("Triangle Connectivity CutOff", Range(0, 100)) = 5
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert alpha
        #pragma target 3.5

        sampler2D _CurrentDepthTexture;

        sampler2D _MainTex;
        float4 _MainTex_ST;

        #define ARCORE_DEPTH_SCALE 0.001        // mm to m
        #define ARCORE_MAX_DEPTH_MM 8191.0
        #define ARCORE_FLOAT_TO_5BITS 31        // (0.0, 1.0) -> (0, 31)
        #define ARCORE_FLOAT_TO_6BITS 63        // (0.0, 1.0) -> (0, 63)
        #define ARCORE_RGB565_RED_SHIFT 2048    // left shift 11 bits
        #define ARCORE_RGB565_GREEN_SHIFT 32    // left shift 5 bits
        #define ARCORE_BLEND_FADE_RANGE 0.01

        struct Input
        {
            float4 vertex;
            float4 customColor;
            float normalizedDepth;
            int clipValue;
            float2 depthTexuv;
        };

        struct VInput
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            uint id : SV_VertexID;
        };

        static const float kClipOutValue = -10000000;
        uniform half _Glossiness;
        uniform half _Metallic;
        uniform fixed4 _Color;
        uniform int _DepthPixelSkipping;
        uniform float _FocalLengthX;
        uniform float _FocalLengthY;
        uniform float _PrincipalPointX;
        uniform float _PrincipalPointY;
        uniform int _ImageDimensionsX;
        uniform int _ImageDimensionsY;
        uniform float _TriangleConnectivityCutOff;
        uniform float _NormalizedDepthMin;
        uniform float _NormalizedDepthMax;
        uniform float4x4 _VertexModelTransform;

        // Adds instancing support for this shader. You need to check
        // 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more
        // information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        inline float ArCoreDepth_GetMeters(float2 uv)
        {
            // The depth texture uses TextureFormat.RGB565.
            float4 rawDepth = tex2Dlod(_CurrentDepthTexture, float4(uv, 0, 0));
            // rawDepth /= 5.0;
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

        void vert(inout VInput v, out Input OUT)
        {
            UNITY_INITIALIZE_OUTPUT(Input, OUT);

            float width = (float)_ImageDimensionsX;
            float height = (float)_ImageDimensionsY;

            uint dimX = (uint)_ImageDimensionsX / (uint)_DepthPixelSkipping;

            float2 texID = int3((uint)v.id % dimX, (uint)v.id / dimX, 0);
            texID.xy *= (float)_DepthPixelSkipping;

            float2 depthTexuv0 = float2(texID.x / width, texID.y / height);
            float2 depthTexuv1 = float2((texID.x + 1) / width, texID.y / height);
            float2 depthTexuv2 = float2(texID.x / width, (texID.y / height + 1));
            float2 depthTexuv3 = float2((texID.x + 1) / width, (texID.y / height + 1));

            // depthTexuv0.xy = -depthTexuv0.yx;
            // depthTexuv1.xy = -depthTexuv1.yx;
            // depthTexuv2.xy = -depthTexuv2.yx;
            // depthTexuv3.xy = -depthTexuv3.yx;

            OUT.depthTexuv = depthTexuv0;
            float4 depths;

            // Skips this vertex if it doesn't have a depth value.
            depths[0] = ArCoreDepth_GetMeters(depthTexuv0);
            if (depths[0] == 0)
            {
                v.vertex = 0;
                //v.normal = 0;
                OUT.clipValue = kClipOutValue;
                OUT.customColor = float4(0, 0, 0, 1);
                OUT.normalizedDepth = 0;
                return;
            }

            depths[1] = ArCoreDepth_GetMeters(depthTexuv1);
            depths[2] = ArCoreDepth_GetMeters(depthTexuv2);
            depths[3] = ArCoreDepth_GetMeters(depthTexuv3);

            // Tests the difference between each of the depth values and the
            // average.
            // If any deviates by the cutoff or more, don't render this triangle.
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
                //v.normal = 0;
                OUT.clipValue = kClipOutValue;
                OUT.customColor = float4(0, 0, 0, 1);
                OUT.normalizedDepth = 0;
            }
            else
            {
                // Calculates vertex positions of right and bottom neighbors.
                v.vertex = GetVertex(texID.x, texID.y, depths[0]);
                float4 vertexRight = GetVertex(texID.x + 1, texID.y, depths[1]);
                float4 vertexBottom = GetVertex(texID.x, texID.y + 1, depths[2]);

                // Calculates the vertex normal.
                float3 sideBA = vertexRight - v.vertex;
                float3 sideCA = vertexBottom - v.vertex;
                float3 normal = normalize(cross(sideBA, sideCA));

                v.normal = normal;
                OUT.clipValue = 0;

                // Normal mapped to color value range.
                OUT.customColor = float4((v.normal + 1) * 0.5, 1);

                float depthRange = _NormalizedDepthMax - _NormalizedDepthMin;
                OUT.normalizedDepth = (depths[0] - _NormalizedDepthMin) / depthRange;

                OUT.customColor = half4( OUT.normalizedDepth, 0.0, 0.0, 1.0);
            }

            OUT.vertex = UnityObjectToClipPos(v.vertex);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float width = (float)_ImageDimensionsY / (float)_DepthPixelSkipping;
            float height = (float)_ImageDimensionsX / (float)_DepthPixelSkipping;

            float4 tex = tex2D(_MainTex, IN.depthTexuv * float2(width, height));
            float clipT = tex.r > 0.5 ? 1 : -1;

            clip(IN.clipValue + clipT);

            // tex2Dlod(_CurrentDepthTexture, float4(IN.depthTexuv, 0, 0));
            // float4(IN.depthTexuv, 0.0, 1.0);
            //float4(TurboColormap(IN.normalizedDepth * 0.95), 1);

            float4 color = _Color * tex;
            o.Albedo = color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
