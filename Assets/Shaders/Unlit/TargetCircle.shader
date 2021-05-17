Shader "Jam3/Unlit/TargetCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        _Radius ("Radius", Range(0,1)) = 1
        _Smoothness ("Smoothness", Range(0, 0.5)) = 0.05

        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha
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

            //Texture
            sampler2D _MainTex;
            float4 _MainTex_ST;

            //Variables
            float4 _Color;
            float _Alpha;
            float _Radius;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed dist = distance(i.uv, float2(0.5, 0.5));
                fixed radius = 1.0 - _Radius;

                fixed toAdd = smoothstep(0.66, 0.659, 1.0 - dist) * 1.5;
                fixed toRemove = smoothstep(0.695, 0.69, 1.0 - dist);

                fixed4 finalcolor = tex2D(_MainTex, i.uv) * _Color;
                finalcolor.a = smoothstep(radius, radius + _Smoothness, 1.0 - dist);
                finalcolor.a *= _Alpha + toAdd - toRemove;
                finalcolor.a = clamp(finalcolor.a, 0.0, 1.0);

                return finalcolor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
