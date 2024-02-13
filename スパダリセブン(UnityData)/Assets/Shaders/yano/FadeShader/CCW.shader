Shader "Unlit/CCW"
{
    Properties
    {
        _Fade("_Fade",float) = 1
       _MaxAlpha("_MaxAlpha",float) = 1
       _MinAlpha("_MinAlpha",float) = 0
       _Texture("Texture",2D) = "white" {}

    //Config
    [HideInInspector]_Div("Division Size", Int) = 10
    }
        SubShader
    {
        Tags
        {
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

            //頂点シェーダーへ入力するデータ構造体
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

    //フラグメントシェーダーへ渡すデータ構造体
   struct v2f
   {
       float2 uv : TEXCOORD0;
       float4 vertex : SV_POSITION;
       fixed4 color : COLOR;
   };

   float rectangle(float2 p, float2 size) {
       return max(abs(p.x) - size.x, abs(p.y) - size.y);
   }

   float trs(float2 p, float val, float div, float t)
   {
       float mn = 0.001;
       float u = 1.0;
       for (int i = 0; i < t; i++) {
           u += (div * 2.0 - 4.0 * i - 2.0) * 4.0;
       }

       float r = (div * 2.0 - 4.0 * t - 2.0);
       float sc = val - u;

       float a = 1;

       float rect = rectangle(p - float2(-div + t * 2.0, -div + t * 2.0 + 1.0), float2(sc, mn));
       a = 1 - step(1.0, rect);

       rect = rectangle(p - float2(div - t * 2.0 - 1.0, -div + (t + 1.0) * 2.0), float2(mn, sc - r - 2.0));
       a = max(a, 1 - step(1.0, rect));

       rect = rectangle(p - float2(div - (t + 1) * 2.0, div - t * 2.0 - 1.0), float2(sc - r * 2.0 - 2.0, mn));
       a = max(a, 1 - step(1.0, rect));

       rect = rectangle(p - float2(-div + t * 2.0 + 1.0, div - (t + 1) * 2.0), float2(mn, sc - r * 3.0 - 2.0));
       a = max(a, 1 - step(1.0, rect));

       return a;
   }

   float _Fade;
   float _MaxAlpha;
   float _MinAlpha;

   float _Div;


   sampler2D _MainTex;
   float4 _MainTex_ST;


   //頂点シェーダー関数
   v2f vert(appdata v)
   {
       v2f o;
       o.vertex = UnityObjectToClipPos(v.vertex);
       o.uv = v.uv;
       o.color = v.color;
       return o;
   }

   //フラグメントシェーダー関数
   fixed4 frag(v2f i) : SV_Target
   {
       float div = _Div;
       float val = _Fade * 200;
       float2 f_st = i.uv * 2.0 - 1.0;
       f_st.x *= -1;
       float a;
       f_st *= div;
       for (int i = 0; i < div * 0.5; i++) {
           a = min(a + trs(f_st, val, div, i), 1);
       }

       fixed4 col = 0.0;
       col.a = (a == 1) ? _MaxAlpha : _MinAlpha;
       return col;
   }
   ENDCG
}
    }
}
