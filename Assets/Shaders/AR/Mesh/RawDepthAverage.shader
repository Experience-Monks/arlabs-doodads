Shader "Jam3/AR/Unlit/RawDepthAverage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 depthTexture = tex2Dlod(_MainTex, float4(depthUv, 0, 0));
                depthTexture.rgb = dot(depthTexture.rgb, float3(0.3, 0.59, 0.11));

                depthTexture.r = depthTexture.r > 0.0 ? 1.0 : 0.0;
                depthTexture.g = depthTexture.g > 0.0 ? 1.0 : 0.0;
                depthTexture.b = depthTexture.b > 0.0 ? 1.0 : 0.0;
                depthTexture.a = 1.0;

                return depthTexture;
            }
            ENDCG
        }
    }
}
