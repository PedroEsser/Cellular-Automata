Shader "CA/CA_2DShader"
{
    Properties
    {
        _WindowSize ("Window Size", Vector) = (0, 0, 0, 0)
        _Position ("Position", Vector) = (0, 0, 0, 0)
        _Scale ("Scale", Float) = 1
        _GridDimensions ("Grid Dimensions", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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

            StructuredBuffer<uint> _GridBuffer;
            float2 _Position;
            float _Scale;
            int4 _GridDimensions;
            float2 _WindowSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = ((v.uv - .5) * _Scale * _WindowSize.xy / _WindowSize.x + _Position);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int2 gridPos = int2(floor(i.uv.xy));
                if(gridPos.x < 0 || gridPos.x >= _GridDimensions.x || gridPos.y < 0 || gridPos.y >= _GridDimensions.y) {
                    return 0.5;
                }

                float2 cellPos = frac(i.uv.xy); // position within the cell (0 to 1)
                float lineWidth = clamp(0.001 * _Scale, 0.0005, 0.01); // adjust thickness based on zoom
                float edgeAA = fwidth(cellPos.x); // screen-space derivative for anti-aliasing

                float lineX = smoothstep(lineWidth, lineWidth + edgeAA, cellPos.x) * 
                            smoothstep(1.0 - lineWidth, 1.0 - lineWidth - edgeAA, cellPos.x);

                float lineY = smoothstep(lineWidth, lineWidth + edgeAA, cellPos.y) * 
                            smoothstep(1.0 - lineWidth, 1.0 - lineWidth - edgeAA, cellPos.y);

                float gridLine = 1.0 - min(lineX, lineY); // 1 = inside line, 0 = cell body

                //int cellIndex = gridPos.x + gridPos.y * _GridDimensions.x;
                int bitIndex = (gridPos.x + gridPos.y * _GridDimensions.w);
                int intIndex = bitIndex / 32 + gridPos.y * _GridDimensions.z;
                bitIndex %= 32;
                int cellValue = (_GridBuffer[intIndex] >> bitIndex) & 1;
                return lerp(cellValue, 1.0, gridLine);
                //return cellValue;
            }
            ENDCG
        }
    }
}
