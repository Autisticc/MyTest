Shader "UI/StencilWrite"
{
    // 用途：挂到"被高亮的任意形状 UI"上。
    // 功能：正常显示自身图形（sprite），同时把覆盖到的不透明像素写入模板缓冲（Ref=1），
    //       供全屏遮罩（UI/HoleMask）做反向模板测试，从而"抠出"本图形的形状作为镂空区。
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        // 关键：把这个物体覆盖的不透明像素的模板值写成 1
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
            Fail Keep
            ZFail Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.color    = v.color * _Color;
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                // 丢弃透明像素：避免图形外的透明区也被写入模板，导致镂空变成"矩形"而非"图形"
                clip(col.a - 0.5);
                return col;
            }
            ENDCG
        }
    }
}
