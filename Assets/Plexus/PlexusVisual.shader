Shader "anduni/PlexusVisual"
{
    Properties
    {
        _Thick("Thickness", Float) = 0.0025
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo
            #include "UnityCG.cginc"

            int _count;
            StructuredBuffer<float3> _vertexBuffer;
            StructuredBuffer<int> _connectionsBuffer;
            float _Thick;

            struct appdata
            {
                float4 vertex : POSITION;
                uint id : SV_VertexID;
            };

            struct vert_out
            {
                float4 vertex : SV_POSITION;
                uint id : FLOAT;
            };

            struct geo_out 
            {
                float4 vertex : POSITION;
            };

            // # kernels
            // ## vertex
            vert_out vert (appdata v)
            {
                vert_out o;
                o.vertex = v.vertex;
                o.id = v.id;
                return o;
            }



            void Connect(float3 src, float3 dst, inout TriangleStream<geo_out> stream) {
                float4 p1 = UnityObjectToClipPos(float4(src, 1));
                float4 p2 = UnityObjectToClipPos(float4(dst, 1));

                float distance = length(dst - src);
                if (distance <= 0) return;

                float2 dir = normalize(p2.xy - p1.xy);
                float2 normal = float2(-dir.y, dir.x);

                float4 offset1 = float4(normal * p1.w * _Thick, 0, 0);
                float4 offset2 = float4(normal * p2.w * _Thick, 0, 0);

                float4 o1 = p1 + offset1;
                float4 o2 = p1 - offset1;
                float4 o3 = p2 + offset2;
                float4 o4 = p2 - offset2;

                geo_out g[4];
                g[0].vertex = o1;
                g[1].vertex = o2;
                g[2].vertex = o3;
                g[3].vertex = o4;

                stream.Append(g[0]);
                stream.Append(g[1]);
                stream.Append(g[2]);
                stream.Append(g[3]);

                stream.RestartStrip();
            }



            // ## geometry
            [maxvertexcount(4)]
            void geo(point vert_out input[1], inout TriangleStream<geo_out> stream)
            {
                float3 vertex = input[0].vertex;
                int id = input[0].id;
                Connect(vertex, _vertexBuffer[_connectionsBuffer[id]], stream);
            }

            // ## fragment
            fixed4 frag (geo_out i) : SV_Target
            {
                // ### default output to white
                return 1;
            }
            ENDHLSL
        }
    }

}
