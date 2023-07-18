// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "SpringGridUpdateSprings"
{
  SubShader
  {
      Pass
      {
          ZTest Always Cull Off ZWrite Off
          Fog { Mode off }

          CGPROGRAM
          #include "UnityCG.cginc"
          #pragma vertex vert
          #pragma fragment frag

          uniform sampler2D _Positions;
          uniform sampler2D _Velocities;
          uniform float4 _ForcePosition[100];
          uniform float4 _ForceDirection[100];
          uniform float4 _ForceRadialMagnitudeRadiusForwardbias[100];  // 4 params in the xyzw here
          uniform int _NumForces;
          uniform int _XPoints;
          uniform int _ZPoints;
          uniform float _SpringK;
          uniform float _SpringDamping;
          uniform float _Mass;
          uniform float _DeltaTime;
          uniform float4 _BoundsMin;
          uniform float4 _BoundsMax;
          uniform float4 _GridSpacing;

          struct v2f {
              float4  pos : SV_POSITION;
              float2  uv : TEXCOORD0;
          };

          struct f2a {
              float4 pos : COLOR0;
              float4 vel : COLOR1;
          };

          int2 GridPos(float2 uv) {
            // UV coords are for the center of the cell. We want to extract the cell indices.
            // UV for cell (i, j) will be ( (2i+1)/2W , (2j+1)/2H ) or ( (i+.5)/W, (j+.5)/H )
            return round(uv * float2(_XPoints, _ZPoints) - .5);
          }
          float2 GridUV(int2 gridPos) {
            // Reverse above. (i,j) => ((i+.5)/W, (j+.5)/H)
            return float2(gridPos + .5) / float2(_XPoints, _ZPoints);
          }
          float3 GridToWorld(int2 gridPos) {
            float2 xz = _BoundsMin.xz + gridPos * _GridSpacing;
            return float3(xz.x, 0, xz.y);
          }

          float3 NeighborF(float3 p, int2 n) {
            if (n.x < 0 || n.x >= _XPoints || n.y < 0 || n.y >= _ZPoints)
              return -_SpringK * (p - GridToWorld(n));  // pretend there's a fixed neighbor for the edge vertices
            float4 np = tex2D(_Positions, GridUV(n));
            return -_SpringK * (p - np.xyz);
          }

          void IterateSprings(inout f2a data, int2 gridPos, float3 accel) {
            float3 p = data.pos.xyz;
            float3 anchor = GridToWorld(gridPos);
            float3 springF = -_SpringK * (p - anchor);
            float3 dampingF = _SpringDamping * data.vel.xyz;
            float2 n = float2(1, 0);
            float3 neighborF = NeighborF(p, gridPos - n.xy) + NeighborF(p, gridPos + n.xy) + NeighborF(p, gridPos - n.yx) + NeighborF(p, gridPos + n.yx);
            float3 force = neighborF + springF - dampingF;
            accel += force / _Mass;
            data.vel.xyz += _DeltaTime * accel;
            data.pos.xyz += _DeltaTime * data.vel.xyz;
          }

          float sqrMagnitude(float3 a) { return dot(a, a); }
          float3 safeNormalize(float3 a) {
            float mag = length(a);
            if (mag == 0) return 0;
            return a / mag;
          }
          float3 ApplyForce(inout f2a data, int i) {
            float3 forcePos = _ForcePosition[i].xyz;
            float3 towardsPoint = data.pos.xyz - forcePos;
            float4 rmrb = _ForceRadialMagnitudeRadiusForwardbias[i];
            bool radial = rmrb.x > 0;
            float magnitude = rmrb.y;
            float radius = rmrb.z;
            float forwardBias = rmrb.w;
            if (sqrMagnitude(towardsPoint) > radius * radius)
              return float3(0, 0, 0);

            float3 baseF;
            if (radial) {
              baseF = safeNormalize(towardsPoint);
            } else {
              float3 forward = _ForceDirection[i].xyz;
              float3 tangent = cross(forward, float3(0, 1, 0));
              bool rightSide = dot(towardsPoint, tangent) > 0;
              if (!rightSide) tangent = -tangent;
              baseF = safeNormalize(lerp(tangent, forward, forwardBias));
            }
            float distanceFactor = 1.0f - (length(towardsPoint) / radius);
            return magnitude * distanceFactor * baseF / _Mass;
          }

          float3 ApplyForces(inout f2a data) {
            float3 accel;
            for (int i = 0; i < _NumForces; i++) {
              accel += ApplyForce(data, i);
            }
            return accel;
          }

          void BoundPosition(inout f2a data, int2 gridPos) {
            if (gridPos.x == 0 || gridPos.y == 0 || gridPos.x == _XPoints - 1 || gridPos.y == _ZPoints - 1) {
              data.pos.xyz = GridToWorld(gridPos);
            } else {
              data.pos.xyz = clamp(data.pos.xyz, _BoundsMin, _BoundsMax);
            }
          }
 
          v2f vert(appdata_base v) {
            v2f OUT;
            OUT.pos = UnityObjectToClipPos(v.vertex);
            OUT.uv = v.texcoord.xy;
            return OUT;
          }

          f2a frag(v2f IN) {
            f2a OUT;
            int2 gridPos = GridPos(IN.uv);
            float2 gridUV = GridUV(gridPos);
            OUT.pos = tex2D(_Positions, gridUV);
            OUT.vel = tex2D(_Velocities, gridUV);

            //OUT.pos.xyz = GridToWorld(gridPos);
            //OUT.vel.xyz = IN.pos;
            float3 accel = ApplyForces(OUT);
            IterateSprings(OUT, gridPos, accel);
            BoundPosition(OUT, gridPos);

            return OUT;
          }

          ENDCG
      }
  }
}