Shader "ReV3nus/HDRPDepth"
{
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Off
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 cameraSpacePosition: TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);

                float4 cameraSpacePosition = float4(UnityObjectToViewPos(v.vertex), 1);
                cameraSpacePosition.z = -cameraSpacePosition.z;
                o.cameraSpacePosition = cameraSpacePosition;

                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float d = i.cameraSpacePosition.z;
                d /= 20.0;
                //return float4(1,1,1,1);
                return float4(d, d, d, 1);
            }
            ENDHLSL
        }
    }
}