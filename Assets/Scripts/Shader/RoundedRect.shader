Shader "Custom/UI/RoundedRect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 centeredUV : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _CornerRadius; // (右上, 右下, 左上, 左下)
            float2 _RectSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // UV 0~1 转换为以中心为原点、以像素为单位的坐标
                o.centeredUV = (v.uv - 0.5) * _RectSize;
                o.color = v.color;
                return o;
            }

            // SDF 圆角矩形
            float sdRoundBox(float2 p, float2 b, float4 r)
            {
                // r = (右上, 右下, 左上, 左下)
                r.x = (p.y > 0.0) ? r.x : r.y;  // 上半取右上, 下半取右下
                r.zw = (p.y > 0.0) ? r.zw : r.wz; // 上半取左上, 下半取左下
                float leftR  = (p.x < 0.0) ? r.z : r.x;
                float rightR = (p.x < 0.0) ? r.w : r.y;
                float radius = (p.x < 0.0) ? leftR : rightR;

                float2 q = abs(p) - b + radius;
                return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - radius;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 halfSize = _RectSize * 0.5;
                float dist = sdRoundBox(i.centeredUV, halfSize, _CornerRadius);
                float alpha = smoothstep(1.0, -1.0, dist);

                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
