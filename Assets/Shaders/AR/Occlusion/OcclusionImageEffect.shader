//-----------------------------------------------------------------------
// <copyright file="OcclusionImageEffect.shader" company="Google LLC">
//
// Copyright 2020 Google LLC
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

Shader "Jam3/AR/Occlusion Image Effect"
{
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _UvTopLeftRight ("UV of top corners", Vector) = (0, 1, 1, 1)
        _UvBottomLeftRight ("UV of bottom corners", Vector) = (0 , 0, 1, 0)

        _OcclusionTransparency ("Maximum occlusion transparency", Range(0, 1)) = 1
        _OcclusionOffsetMeters ("Occlusion offset [meters]", Float) = 0
        _TransitionSize ("Occlusion transition range", Float) = 0.05

        _MaximumOcclusionDistance ("Maximum occlusion distance", Float) = 4
        _MaximumOcclusionDistanceTransition ("Maximum occlusion distance transition width", Float) = 0.3
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "ForceNoShadowCasting" = "True"
        }

        Cull Off ZWrite Off ZTest Always

        CGINCLUDE
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

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }
        ENDCG

        // Pass #0 renders an auxiliary buffer - occlusion map that indicates the
        // regions of virtual objects that are behind real geometry.
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CameraDepthTexture;

            sampler2D _CurrentDepthTexture;
            float4x4 _DisplayRotationPerFrame;

            sampler2D _BackgroundTexture;

            float _TransitionSize;
            float _MaximumOcclusionDistance;
            float _MaximumOcclusionDistanceTransition;
            float _OcclusionOffsetMeters;

            half4 frag (v2f i) : SV_Target
            {
                float2 depthUv = mul(float3(i.uv, 1.0f), _DisplayRotationPerFrame).xy;
                float depthMeters = tex2Dlod(_CurrentDepthTexture, float4(depthUv, 0, 0));

                float virtualDepthMeters = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) - _OcclusionOffsetMeters;

                // Far plane minus near plane.
                float maxVirtualDepth = _ProjectionParams.z - _ProjectionParams.y;

                float halfTransitionRangeMeters = virtualDepthMeters * _TransitionSize * 0.5;

                // 0.0 - fully visible, 1.0 - fully occluded.
                float occlusionAlpha = smoothstep(depthMeters - halfTransitionRangeMeters, depthMeters + halfTransitionRangeMeters, virtualDepthMeters);

                // 0.0 - always visible and far, 1.0 - occludable and close.
                float distanceAlpha = smoothstep(_MaximumOcclusionDistance + _MaximumOcclusionDistanceTransition, _MaximumOcclusionDistance,virtualDepthMeters);
                occlusionAlpha = min(occlusionAlpha, distanceAlpha);

                // Masks out only the fragments with virtual objects.
                occlusionAlpha *= saturate(maxVirtualDepth - virtualDepthMeters);

                // At this point occlusionAlpha is equal to 1.0 only for fully
                // occluded regions of the virtual objects.
                half4 background = tex2D(_BackgroundTexture, i.uv);

                return half4(background.rgb, occlusionAlpha);
            }
            ENDCG
        }

        // Pass #1 combines virtual and real cameras based on the occlusion map.
        Pass
        {
            Cull Off
            ZTest Always
            ZWrite On
            Lighting Off
            LOD 100

            Tags
            {
                "LightMode" = "Always"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _OcclusionMap;
            fixed _OcclusionTransparency;
            fixed _OcclusionDisabled;
            fixed _CorrectUV;

            float2 getAspectUV(float2 uv)
            {
                float aspect = _ScreenParams.y / _ScreenParams.x;
                float2 aspecUV = uv;
                aspecUV.x -= 0.5;
                aspecUV.x /= aspect;
                aspecUV.x += 0.5;
                return aspecUV;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 aspecUV = _CorrectUV > 0.5 ? getAspectUV(i.uv) : i.uv;

                half4 foreground = tex2D(_MainTex, i.uv);
                half4 occlusionMap = tex2D(_OcclusionMap, aspecUV);
                half4 background = half4(occlusionMap.rgb, 1.0);

                // `occlusion` equal to 1 means fully occluded object.
                float occlusion = occlusionMap.a;

                // Clips occlusion if we want to partially show occluded object.
                occlusion = min(occlusion, _OcclusionTransparency);
                return _OcclusionDisabled > 0.5 ? foreground : lerp(foreground, background, occlusion);
            }

            ENDCG
        }
    }
}
