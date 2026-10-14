Shader "UI/HoleMask"
{
    // 用途：全屏引导遮罩。挂在全屏 Image 上。
    // 功能：在"模板值 != 1"的像素上画一层半透明黑色，即把没有被 StencilWrite 标记的,不等于1就是透明，相当于之前学不规则按钮时候的技术。
    //       区域遮挡住；被标记（=1）的区域（即那个任意形状的按钮）不画黑，形成镂空高亮。
    Properties
    {
        // UGUI 的 CanvasRenderer 强制要求材质必须有 _MainTex 槽，即使本 shader 不采样，也需声明
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Mask Color", Color) = (0,0,0,0.6)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        // 关键：只在模板值 != 1 的地方画黑（=1 处是按钮形状，跳过）
        Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
            Fail Keep
            ZFail Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
