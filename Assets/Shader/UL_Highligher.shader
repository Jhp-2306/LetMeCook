Shader "Unlit/UL_Highlighter_Local"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _HighlightColor ("Highlight Color", Color) = (1,1,0,1)
        _HighlightIntensity ("Highlight Intensity", Range(0,1)) = 0
        _MinY ("Min Y (Local)", Float) = 0
        _MaxY ("Max Y (Local)", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _HighlightColor;
        float _HighlightIntensity;
        float _MinY;
        float _MaxY;

        struct Input
        {
            float2 uv_MainTex;
            float localY;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.uv_MainTex = v.texcoord;
            o.localY = v.vertex.y; // Local Y from object space
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Base texture and highlight blending
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            tex.rgb = lerp(tex.rgb, _HighlightColor.rgb, _HighlightIntensity);

            // Gradient alpha based on local space Y
            float gradient = saturate((IN.localY - _MinY) / (_MaxY - _MinY));

            o.Albedo = tex.rgb;
            o.Alpha = tex.a * gradient;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
