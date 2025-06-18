Shader "Unlit/UL_Highligher"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _HighlightColor ("Highlight Color", Color) = (1,1,0,1)
        _HighlightIntensity ("Highlight Intensity", Range(0,1)) = 0
        _MinY ("Min Y", Float) = 0
        _MaxY ("Max Y", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _HighlightColor;
        float _HighlightIntensity;
        float _MinY;
        float _MaxY;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Sample base texture and color
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Apply highlight blending
            tex.rgb = lerp(tex.rgb, _HighlightColor.rgb, _HighlightIntensity);

            // Calculate vertical gradient alpha based on Y position
            float gradient = saturate((IN.worldPos.y - _MinY) / (_MaxY - _MinY));

            o.Albedo = tex.rgb;
            o.Alpha = tex.a * gradient;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         LOD 100

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             // make fog work
//             #pragma multi_compile_fog

//             #include "UnityCG.cginc"

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 UNITY_FOG_COORDS(1)
//                 float4 vertex : SV_POSITION;
//             };

//             sampler2D _MainTex;
//             float4 _MainTex_ST;

//             v2f vert (appdata v)
//             {
//                 v2f o;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                 UNITY_TRANSFER_FOG(o,o.vertex);
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 // sample the texture
//                 fixed4 col = tex2D(_MainTex, i.uv);
//                 // apply fog
//                 UNITY_APPLY_FOG(i.fogCoord, col);
//                 return col;
//             }
//             ENDCG
//         }
//     }
// }
