Shader "Jam3/Unlit/BoundsLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "ForceNoShadowCasting" = "True" "IgnoreProjector" = "True"}
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha nometa nodynlightmap noforwardadd
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                clip(_Alpha - 0.05);

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a = _Alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
