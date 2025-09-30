Shader "Unlit/UL_ProgressBar"
{
   Properties
    {
        _ColorGreen("NonSafe Color", Color) = (0, 1, 0, 1)
        _ColorRed("Safe Color", Color) = (1, 0, 0, 1)
        _RedZoneCenter("Red Zone Center (0 to 1)", Range(0,1)) = 0
        _RedZoneWidth("Red Zone Width (0 to 1)", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        //LOD 100

        Pass
        {
             ZWrite Off
             Cull Off
             Lighting Off
             Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _ColorGreen;
            fixed4 _ColorRed;
            float _RedZoneCenter;
            float _RedZoneWidth;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //float redZoneStart = _RedZoneCenter;
                float redZoneEnd = _RedZoneCenter + _RedZoneWidth;
                
                if (i.uv.x >= _RedZoneCenter && i.uv.x <= redZoneEnd)
                {
                    return _ColorRed;
                }

                return _ColorGreen;
            }
            ENDCG
        }
    }
}
