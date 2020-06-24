Shader "_dwmi/point-visual"
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
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #include "UnityCG.cginc"

            
            StructuredBuffer<float3> _computeBuffer;
            float4 _vertexBuffer[125];
            int _vertexCount;

            float _Thick;

            struct appdata
            {
                float4 vertex : POSITION;
                uint id : SV_VertexID;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
            };

            struct g2f
            {
                float4 vertex : POSITION;
            };

            v2g vert(appdata v) 
            {
                v2g o;
                o.vertex = v.vertex;
                return o;
            }


            void Connect (float3 src, float3 dst, inout TriangleStream<g2f> stream) {
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

                g2f g[4];
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

            

            // only allow geo for 3 connections
            [maxvertexcount(256)]
            void geom (point v2g input[1], inout TriangleStream<g2f> stream)
            {
                float3 vertex = input[0].vertex;
                for (int i = 0; i < _vertexCount; i++) {
                    Connect(vertex, _vertexBuffer[i].xyz, stream);
                }
            }


            fixed4 frag (g2f i) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
