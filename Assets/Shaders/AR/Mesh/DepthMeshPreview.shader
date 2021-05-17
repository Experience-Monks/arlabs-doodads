Shader "Jam3/AR/DepthMeshPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalizedDepthMin ("Normalized Depth Min", Range(0,5)) = 0.3
        _NormalizedDepthMax ("Normalized Depth Max", Range(0,10)) = 4

        _TriangleConnectivityCutOff ("TriangleConnectivityCutOff", Range(0,100)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                uint id : SV_VertexID;
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

            //Variables
            uniform int _ImageDimensionsX;
            uniform int _ImageDimensionsY;

            v2f vert (appdata v)
            {
                v2f o;

                float2 texID = int3((uint)v.id % (uint)_ImageDimensionsX, (uint)v.id / (uint)_ImageDimensionsX, 0);
                float2 depthUV = float2(texID.x / (float)_ImageDimensionsX, texID.y / (float)_ImageDimensionsY);
                o.uv = depthUV.xy;

                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // col.a = 0.5;
                return col;
            }
            ENDCG
        }
    }
}
