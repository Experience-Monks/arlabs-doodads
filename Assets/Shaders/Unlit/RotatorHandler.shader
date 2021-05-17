Shader "Jam3/Unlit/RotatorHandler"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)

        _InsideRadius ("Inside Radius", Range(0,1)) = 0.3
        _OutsideRadius ("Outside Radius", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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

            //Variables
            float4 _Color;
            float _InsideRadius;
            float _OutsideRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed dist = distance(i.uv, float2(0.5, 0.5));
                fixed outRadius = 1.0 - _OutsideRadius;
                fixed innerRadius = 1.0 - _InsideRadius;

                float mask = smoothstep(outRadius, outRadius + 0.001, 1.0 - dist);
                mask -= smoothstep(innerRadius, innerRadius + 0.001, 1.0 - dist);
                clip(mask - 0.01);

                return _Color;
            }
            ENDCG
        }
    }
}
