// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "SpringGridRenderLines"
{
  Properties
  {
      _MainTex("Source Texture", 2D) = "white" {}
  }

  SubShader
  {
      Pass
      {
          Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

          Blend SrcAlpha OneMinusSrcAlpha
          ZTest Always Cull Off ZWrite Off
          Fog { Mode off }

          CGPROGRAM
          #pragma multi_compile_instancing
          #include "UnityCG.cginc"
          #pragma vertex vert
          #pragma fragment frag

          UNITY_INSTANCING_BUFFER_START(Props)
             UNITY_DEFINE_INSTANCED_PROP(float, _StartIndex)
             UNITY_DEFINE_INSTANCED_PROP(float, _EndIndex)
             //UNITY_DEFINE_INSTANCED_PROP(float4, _BasePosition)
          UNITY_INSTANCING_BUFFER_END(Props)

          uniform sampler2D _MainTex;
          uniform sampler2D _Positions;
          uniform int _XPoints;
          uniform int _ZPoints;
          uniform float _LineHalfWidth;
          uniform float _FatLineHalfWidth;
          uniform float4 _Color;
          uniform float4 _FatColor;
          uniform float4 _BoundsMin;
          uniform float4 _BoundsMax;
          uniform float4 _GridSpacing;
          uniform float4 _Dir;

          struct Attributes
          {
              float3 vertex      : POSITION;
              //float4 color       : COLOR;
              float2 texcoord    : TEXCOORD0;
              UNITY_VERTEX_INPUT_INSTANCE_ID
          };

          struct v2f
          {
              float4 pos : SV_POSITION;
              float4 color : COLOR;
              float2 uv : TEXCOORD0;
          };

          struct f2a
          {
              float4 pos : COLOR0;
          };

          uint2 GridIndex(uint vi) {
            return float2(vi % _XPoints, vi / _XPoints);
          }
          float2 GridUV(uint2 gridPos) {
            return float2(gridPos + .5) / float2(_XPoints, _ZPoints);
          }
          float4 TexIndex(uint2 gridPos) {
            return float4(GridUV(gridPos), 0, 0);
          }

          float3 VertexToPosition(float3 v, uint2 startPoint, uint2 endPoint) {
            float4 start = tex2Dlod(_Positions, TexIndex(startPoint));
            float4 end = tex2Dlod(_Positions, TexIndex(endPoint));
            float4 delta = end - start;
            float2 rv;
            bool fat = _Dir.x > 0 ? (startPoint.y % 4 == 0) : (startPoint.x % 4 == 0);
            float halfWidth = fat ? _FatLineHalfWidth : _LineHalfWidth;
            if (_Dir.x > 0) { // Horizontal
              rv = start.xz + delta.xz * v.xx + float2(0, v.z * halfWidth);
            } else { // Vertical
              rv = start.xz + delta.xz * v.zz + float2(v.x * halfWidth, 0);
            }
            return float3(rv.x, 0, rv.y);
          }

          v2f vert(Attributes v) {
              UNITY_SETUP_INSTANCE_ID(v);

              v2f OUT;
              uint startIndex = UNITY_ACCESS_INSTANCED_PROP(Props, _StartIndex);
              uint endIndex = UNITY_ACCESS_INSTANCED_PROP(Props, _EndIndex);
              uint2 startPoint = GridIndex(startIndex);
              uint2 endPoint = GridIndex(endIndex);
              OUT.pos = UnityObjectToClipPos(VertexToPosition(v.vertex, startPoint, endPoint));
              OUT.uv = v.texcoord.xy;
              bool fat = _Dir.x > 0 ? (startPoint.y % 4 == 0) : (startPoint.x % 4 == 0);
              OUT.color = fat ? _FatColor : _Color;
              return OUT;
          }

          f2a frag(v2f IN) {
              float4 color = tex2D(_MainTex, IN.uv);

              f2a OUT;
              OUT.pos = IN.color * color;

              return OUT;
          }

          ENDCG

      }
  }
}