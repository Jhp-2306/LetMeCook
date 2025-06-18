Shader "Custom/SSS_Outliner"
//Shader "Custom/ToonShaderRoy"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _Ramp ("Ramp Texture", 2D) = "gray" {}
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Ramp;
            fixed4 _Color;

            fixed4 _RimColor;
            float _RimPower;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // Diffuse Lighting
                float NdotL = dot(normal, lightDir);
                float2 rampUV = float2(NdotL * 0.5 + 0.5, 0.5);
                fixed3 ramp = tex2D(_Ramp, rampUV).rgb;

                // Rim Lighting
                float rimDot = 1.0 - saturate(dot(viewDir, normal));
                float rimFactor = pow(rimDot, _RimPower);
                fixed3 rim = rimFactor * _RimColor.rgb;

                fixed3 tex = tex2D(_MainTex, i.uv).rgb;
                fixed3 finalColor = tex * _Color.rgb * ramp + rim;

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
