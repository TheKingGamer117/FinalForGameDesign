// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader"Custom/SimpleOceanShader"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0,1,1,1)
        _BottomColor ("Bottom Color", Color) = (0,0,0,1)
        _WaveHeight ("Wave Height", float) = 0.5
        _WaveSpeed ("Wave Speed", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

            // Declare shader properties
fixed4 _TopColor;
fixed4 _BottomColor;
float _WaveHeight;
float _WaveSpeed;

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 worldPos : TEXCOORD1;
};

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Convert to world position
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // Create a gradient based on the y-coordinate in world space
    float gradient = 1.0 - saturate(i.worldPos.y / 10.0);
    fixed4 color = lerp(_TopColor, _BottomColor, gradient);

                // Add simple wave motion based on time and position
    float wave = sin(i.worldPos.x + _Time.y * _WaveSpeed) * _WaveHeight;
    color *= 1.0 + wave;

    return color;
}
            ENDCG
        }
    }
}
