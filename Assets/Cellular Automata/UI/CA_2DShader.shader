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

            StructuredBuffer<int> _GridBuffer;
            float2 _Position;
            float _Scale;
            float2 _GridDimensions;
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
                float2 gridPos = floor(i.uv.xy);
                if(gridPos.x < 0 || gridPos.x >= _GridDimensions.x || gridPos.y < 0 || gridPos.y >= _GridDimensions.y) {
                    return 0.5;
                }
                //float2 cellPos = frac(i.uv.xy);
                //if(cellPos.x < 0.025 || cellPos.x > 0.975 || cellPos.y < 0.025 || cellPos.y > 0.975) {
                //    return 1;
                //}

                int cellIndex = (int)gridPos.x + (int)gridPos.y * _GridDimensions.x;
                int intIndex = cellIndex / 32;
                int bitIndex = cellIndex % 32;
                int cellValue = (_GridBuffer[intIndex] >> bitIndex) & 1;
                return cellValue;
            }
            ENDCG
        }
    }
}
