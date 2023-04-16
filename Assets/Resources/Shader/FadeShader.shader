Shader "YU_UI/FadeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MyMask("MyMask",Range(0,0.7)) = 0.7
    }
    SubShader
    {
        Tags { "Queue" = "Transparent"}
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 screenUV:TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed _MyMask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenUV = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex,i.uv);
                float2 diagonal = float2(1,1);
                float Cos = dot(diagonal,i.screenUV.xy)/(length(i.screenUV)*length(diagonal));
                float Sin = sqrt(1 - Cos*Cos);

                float Length = length(i.screenUV)*Sin;
                clip(_MyMask - Length-0.02);

                return col;
            }
            ENDCG
        }
    }
}
