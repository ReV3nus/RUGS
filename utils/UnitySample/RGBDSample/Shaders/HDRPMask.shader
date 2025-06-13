Shader "ReV3nus/HDRPMask"
{
    Properties
    {
        _Radius("Detection Radius", Range(0, 100)) = 10.0
        _Center("Detection Center", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="HDRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Radius;
                float3 _Center;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.position = TransformWorldToHClip(o.worldPos);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // ���㵽������ĵľ���
                float3 offset = i.worldPos - _Center;
                offset.y = 0;
                float distanceSqr = dot(offset, offset);
                
                // �ж��Ƿ������η�Χ��
                bool inRange = distanceSqr < 600;
                
                // �����巶Χ�жϣ���ѡ��
                // bool inCubeRange = 
                //     abs(offset.x) < _Radius && 
                //     abs(offset.y) < _Radius && 
                //     abs(offset.z) < _Radius;

                return inRange ? float4(1, 1, 1, 1) : float4(0, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}               