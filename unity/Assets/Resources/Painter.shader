// Shader vẽ 2D cho Painter (vẽ tức thời bằng GL): ảnh × màu đỉnh, chọn cách trộn màu,
// _Silhouette = 1 thì tô cả hình một màu (bóng mờ khi lướt).
Shader "LongThan/Painter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst", Float) = 10
        _Silhouette ("Silhouette", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite Off ZTest Always Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; float _Silhouette;
            struct a2v { float4 pos : POSITION; float4 col : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float4 col : COLOR; float2 uv : TEXCOORD0; };
            v2f vert (a2v v) { v2f o; o.pos = UnityObjectToClipPos(v.pos); o.col = v.col; o.uv = v.uv; return o; }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 t = tex2D(_MainTex, i.uv);
                fixed4 c = lerp(t * i.col, fixed4(i.col.rgb, t.a * i.col.a), _Silhouette);
                return c;
            }
            ENDCG
        }
    }
}
