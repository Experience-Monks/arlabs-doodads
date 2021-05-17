Shader "Jam3/Unlit/ProjectorShadow"
{
    Properties
    {
        _ShadowTex ("Cookie", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,0)
        _Alpha ("Alpha", Range(0,1)) = 0.5
    }

    Subshader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent+100"}
        Pass
        {
            ZWrite Off
            Offset -1, -1

            Fog { Mode Off }

            ColorMask RGB
            Blend OneMinusSrcAlpha SrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_fog_exp2
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            sampler2D _ShadowTex;
            float4x4 unity_Projector;
            float4 _Color;
            float _Alpha;

            v2f vert(appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = mul (unity_Projector, v.vertex);
                return o;
            }

            half4 frag (v2f i) : COLOR
            {
                float falloff = (i.uv.z + 1.6) / 5;
                falloff = saturate(falloff);
                falloff = smoothstep(0.0, 1.0, falloff);

                half4 color = _Color;
                half4 tex = tex2Dproj(_ShadowTex, i.uv);
                color.a = lerp(lerp(tex.r, 1, _Alpha), 1, falloff);
                if (i.uv.w < 0)
                {
                    color = float4(0,0,0,1);
                }
                return color;
            }
            ENDCG

        }
    }
}
