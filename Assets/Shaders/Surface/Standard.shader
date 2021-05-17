Shader "Jam3/Surface/Standard Specular"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2

        [Header(Albedo)]
        [Space(10)]
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        [Space(10)]
        _MainTex ("Base Diffuse Map (RGB)", 2D) = "white" {}

        [Header(Specucular)]
        [Space(10)]
        _SpecularTex ("Specular Map (RGB)", 2D) = "white" {}
        _Specular ("Specular Intensity", Range(0, 1)) = 1.0

        [Header(Roughness)]
        [Space(10)]
        _RoughnessTex ("Roughness Map (RGB)", 2D) = "white" {}
        _Roughness ("Roughness Intensity", Range(-1, 1)) = 0.0

        [Header(Normal)]
        [Space(10)]
        _NormalTex ("Normal Map (RGB)", 2D) = "black" {}
        _Normal ("Normal Intensity", Range(0, 10)) = 1.0

        [Header(Occlusion)]
        [Space(10)]
        [MaterialToggle] _UseOcclusion ("Use Occlusion", Float) = 0.0
        _OcclusionTex ("Occlusion Map (RGB)", 2D) = "white" {}
        _Occlusion ("Occlusion Intensity", Range(0, 2)) = 1.0

        [Header(Emission)]
        [Space(10)]
        [MaterialToggle] _UseEmission ("Use Emission", Float) = 0.0
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionTex ("Emission Map (RGB)", 2D) = "white" {}
        _Emission ("Emission Intensity", Range(0, 10)) = 1.0

        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimIntensity ("Rim Intensity", Range(0.0, 10.0)) = 0.0
        _RimPower ("Rim Power", Range(0.0, 8.0)) = 0.0

        _Amount ("Amount", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "LightMode" = "ForwardBase" "RenderType"="Opaque" }
        LOD 200
        ZWrite On
        Cull [_Cull]

        CGPROGRAM
        #pragma surface surf StandardSpecular vertex:vert fullforwardshadows
        #pragma target 3.5

        fixed4 _Color;
        // Texture 1
        sampler2D _MainTex;

        sampler2D _SpecularTex;
        half _Specular;

        sampler2D _RoughnessTex;
        half _Roughness;

        sampler2D _NormalTex;
        half _Normal;

        sampler2D _OcclusionTex;
        half _UseOcclusion;
        half _Occlusion;

        half _UseEmission;
        fixed4 _EmissionColor;
        sampler2D _EmissionTex;
        half _Emission;

        fixed4 _RimColor;
        half _RimIntensity;
        half _RimPower;

        sampler2D _NoiseTex;

        half _Amount;

        struct Input
        {
            float2 texcoord;
            float4 pos;
            float3 viewDir;
            INTERNAL_DATA
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.pos = v.vertex;
            o.texcoord = v.texcoord.xy;
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));

            half4 specularTex = tex2D(_SpecularTex, IN.texcoord);
            half4 roughtness = tex2D(_RoughnessTex, IN.texcoord);

            half4 emissiveTex = tex2D(_EmissionTex, IN.texcoord);
            half4 emission = _EmissionColor * emissiveTex;

            float occlusion = 1.0;
            if(_UseOcclusion >= 0.5)
            {
                occlusion = tex2D(_OcclusionTex, IN.texcoord).g;
            }

            half4 color = (tex2D(_MainTex, IN.texcoord) * _Color);
            float AO = LerpOneTo(occlusion, _Occlusion);
            half3 rimEmission = _RimColor.rgb * pow (rim, _RimPower) * _RimIntensity;

            half4 gsColor = dot(color, half3(0.3, 0.59, 0.11));
            color = lerp(color, gsColor, _Amount);

            o.Albedo = color * AO;
            o.Specular = specularTex.rgb * _Specular;
            o.Smoothness = saturate(roughtness.r + _Roughness);
            o.Emission = _UseEmission == 0.0 ? 0.0 +  rimEmission: (emission * _Emission) + rimEmission;
            o.Normal = UnpackScaleNormal(tex2D(_NormalTex, IN.texcoord), _Normal);
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
