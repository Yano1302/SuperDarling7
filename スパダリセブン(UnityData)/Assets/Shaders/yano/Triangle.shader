Shader "Unlit/Triangle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PointX("OriginPointX", Range(-0.5, 0.5)) = 0       //視界の始点のUV座標(U)
        _PointY("OriginPointY", Range(-0.5,0.5)) = 0        //視界の始点のUV座標(V)
        _Width("Width", Range(0.0, 3.0)) = 0                //視点の幅
        _Radius("Radius",Range(0.0,0.5)) = 0                 //円の半径
        _InvAlpha("InvisibleAlpha",Range(0.0, 1.0)) = 1.0  //暗い部分のα値(低くするほど薄くなる)
        _vAlpha("VisibleAlpha", Range(0.0, 1.0)) = 0        //見えている部分のα値
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
            fixed _PointX;
            fixed _PointY;
            fixed  _InvAlpha;
            fixed  _vAlpha;
            fixed  _Radius;

         
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
                //UV座標を中心にずらす
                i.uv -= 0.5;
                //補間値を調べる
                fixed delta = _Width / (0.5 - _PointY);
                //補間値から現在の幅の半分を調べる
                fixed halfwidth = (i.uv.y - _PointY) * delta / 2;
                //中心から左右に半分の幅を足し引きして幅を作る
                float2 pos = { _PointX - halfwidth,_PointX + halfwidth };
                //視点の高さがまだ満たしていない場合はくり抜かない
                 fixed alpha = step(i.uv.y,_PointY);
                //高さを満たした幅の中だけくり抜く
                alpha += step(i.uv.x, pos.x);
                alpha += step(pos.y,i.uv.x);
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                alpha *= smoothstep(_Radius, _Radius, length(i.uv));
                col.a = (alpha >= 1)? _InvAlpha : _vAlpha;
                return col;
            }           
            ENDCG
        }
    }
}
