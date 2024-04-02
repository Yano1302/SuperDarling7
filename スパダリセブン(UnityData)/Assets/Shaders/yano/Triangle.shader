Shader "Unlit/Triangle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Point("Point",float) = 0.5
        _Width("Width",float) = 1.0         //éãì_ÇÃïù
        _Range("Range",float) = 1.0         //éãì_ÇÃí∑Ç≥
    }
    SubShader
    {
        Tags { 
             "RenderType" = "Transparent"
             "Queue" = "Transparent"
        }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed _Width;
            fixed _Range;


         
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //åXÇ´Ç©ÇÁï`âÊîÕàÕÇì¡íËÇ∑ÇÈ
                fixed slope = _Range;               
                fixed alpha = step((i.uv.y / slope), i.uv.x);
                alpha += step(i.uv.x,1 -(i.uv.y / slope));
                
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;            
                col.a = clamp(alpha,0,1);
                return col;
            }


           
            ENDCG
        }
    }
}
