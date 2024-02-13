Shader "Unlit/RightToLeft"
{
    Properties
    {
        _Fade("_Fade",float) = 0
       _MaxAlpha("_MaxAlpha",float) = 1
       _MinAlpha("_MinAlpha",float) = 0
       _Texture("Texture",2D) = "white" {}
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

            float _Fade;
            float _MaxAlpha;
            float _MinAlpha;
            sampler2D _MainTex;
            float4 _MainTex_ST;


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                col.a = (_Fade > i.uv.x)?_MaxAlpha:_MinAlpha;
                return col;
            }
            ENDCG
        }
    }
}
