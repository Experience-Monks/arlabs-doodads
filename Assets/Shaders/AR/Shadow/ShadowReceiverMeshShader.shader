Shader "AR/Shadow Receiver Mesh Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowIntensity ("Shadow Intensity", Range (0, 1)) = 0.6
    }

    SubShader
    {
        Tags {"Queue"="Background+1" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 200
        ZWrite On
        Blend Zero SrcColor

        CGPROGRAM
        #pragma surface surf ShadowOnly vertex:vert alphatest:_Cutoff
        #pragma target 3.5

        struct Input
        {
            float2 texcoord;
            float4 pos;
        };

        fixed4 _Color;
        float _ShadowIntensity;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.pos = v.vertex;
            o.texcoord = v.texcoord.xy;
        }

        inline fixed4 LightingShadowOnly(SurfaceOutput s, fixed3 lightDir, fixed attenuation)
        {
            fixed4 c;
            c.rgb = lerp(s.Albedo, s.Albedo * attenuation, _ShadowIntensity);
            c.a = s.Alpha;
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color;
            o.Alpha = 0;
        }
        ENDCG
    }
    Fallback "Transparent/Cutout/VertexLit"
}
