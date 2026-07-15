// Shader "UI/Grayscale"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint", Color) = (1,1,1,1)
//         _GrayscaleAmount ("Grayscale Amount", Range(0,1)) = 1
//     }

//     SubShader
//     {
//         Tags
//         {
//             "Queue"="Transparent"
//             "IgnoreProjector"="True"
//             "RenderType"="Transparent"
//             "PreviewType"="Plane"
//             "CanUseSpriteAtlas"="True"
//         }

//         Cull Off
//         Lighting Off
//         ZWrite Off
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex : POSITION;
//                 float4 color : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex : SV_POSITION;
//                 fixed4 color : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             sampler2D _MainTex;
//             fixed4 _Color;
//             float _GrayscaleAmount;

//             v2f vert(appdata_t v)
//             {
//                 v2f o;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.texcoord = v.texcoord;
//                 o.color = v.color * _Color;
//                 return o;
//             }

//             fixed4 frag(v2f i) : SV_Target
//             {
//                 fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;

//                 float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

//                 col.rgb = lerp(col.rgb, gray.xxx, _GrayscaleAmount);

//                 return col;
//             }
//             ENDCG
//         }
//     }
// }
Shader "UI/Grayscale"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _GrayscaleAmount ("Grayscale Amount", Range(0,1)) = 1

        // Required for Unity UI Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;
            float _GrayscaleAmount;

            v2f vert(appdata_t v)
            {
                v2f o;

                o.worldPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;

                // Grayscale
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(col.rgb, gray.xxx, _GrayscaleAmount);

                // RectMask2D support
                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif

                // Mask support
                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif

                return col;
            }
            ENDCG
        }
    }
}