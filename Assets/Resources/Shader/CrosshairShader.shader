Shader "Custom/CrosshairShader"
{
    Properties
    {
        _CrosshairSize ("Crosshair Size", Float) = 10 // 准星大小
        _CrosshairThickness ("Crosshair Thickness", Float) = 1 // 准星粗细
        _CrosshairColor ("Crosshair Color", Color) = (1,1,1,1) // 准星颜色
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _CrosshairSize;
            float _CrosshairThickness;
            fixed4 _CrosshairColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(_ScreenParams.x / 2, _ScreenParams.y / 2);
                float2 uv = i.pos.xy / i.pos.w;
                float2 dist = abs(uv - center);

                float2 size = float2(_CrosshairSize, _CrosshairThickness);

                float2 d = fwidth(dist);
                float2 inside = smoothstep(size - d, size + d, dist);

                fixed4 color = _CrosshairColor;
                color.a *= min(inside.x, inside.y);
                return color;
            }
            ENDCG
        }
    }
}
