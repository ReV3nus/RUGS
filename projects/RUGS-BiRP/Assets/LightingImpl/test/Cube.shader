Shader "Unlit/CubemapDebug"
{
    Properties
    {
        _CubeMap("CubeMap", Cube) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _CubeMap;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 sampleDir = i.worldNormal;
                //sampleDir.x = -sampleDir.x;
                return texCUBE(_CubeMap, i.worldNormal) /20;
            }
            ENDCG
        }
    }
}
