using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class SpringGrid : MonoBehaviour {
  public MeshFilter Mesh;
  public Vector2 GridSpacing;
  public float SpringK = 2f;
  public float SpringDamping = 10f;
  public float Mass = 1;
  int XPoints, ZPoints;

  [BurstCompile]
  struct SpringData {
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;
  }
  NativeArray<SpringData>[] JobBuffer = new NativeArray<SpringData>[2]; // double buffered data
  int BufferIdx = 0;
  NativeArray<PForce> Forces;
  int NumForces;

  // These 2 must match SpringJob's versions.
  int PointIndex(int x, int z) => z*XPoints + x;
  Vector3 GridToWorld(int x, int z) => new(Bounds.Instance.XMin + x*GridSpacing.x, 0f, Bounds.Instance.ZMin + z*GridSpacing.y);

  void Start() {
    XPoints = (int)(Bounds.Instance.XSize / GridSpacing.x) + 1;
    ZPoints = (int)(Bounds.Instance.ZSize / GridSpacing.y) + 1;

    BuildMesh();

    int numPoints = XPoints * ZPoints;
    JobBuffer[0] = new NativeArray<SpringData>(numPoints, Allocator.Persistent);
    JobBuffer[1] = new NativeArray<SpringData>(numPoints, Allocator.Persistent);
    Forces = new NativeArray<PForce>(500, Allocator.Persistent);

    // Set spring anchor positions.
    for (int z = 0; z < ZPoints; z++) {
      for (int x = 0; x < XPoints; x++) {
        var vi = PointIndex(x, z);
        var p = GridToWorld(x, z);
        JobBuffer[0][vi] = new() { Position = p, Velocity = Vector3.zero };
      }
    }

    Job = MakeJob();
    BufferIdx++;
    JobHandle = Job.Schedule(numPoints, 64);
  }

  float GetLineWidth(int idx) => (idx % 4 == 0) ? FatLineHalfWidth : LineHalfWidth;
  Color GetLineColor(int idx) => (idx % 4 == 0) ? FatLineColor : LineColor;
  public float LineHalfWidth = .25f;
  public float FatLineHalfWidth = .4f;
  public Color LineColor = Color.blue;
  public Color FatLineColor = Color.blue;
  Vector3[] Vertices;
  void BuildMesh() {
    var mesh = new Mesh();
    mesh.name = this.name + " Procedural Mesh";
    mesh.MarkDynamic();
    Mesh.mesh = mesh;

    var vertices = new List<Vector3>();
    var colors = new List<Color>();
    var uvs = new List<Vector2>();
    var triangles = new List<int>();

    void AddVert(Vector3 pos, Vector2 uv, Color color) {
      vertices.Add(pos);
      uvs.Add(uv);
      colors.Add(color);
    }
    void AddLineStart(int x, int z, Vector3 across, Color color) {
      var pos = GridToWorld(x, z);
      AddVert(pos - across, new(0.0f, 0.0f), color);
      AddVert(pos + across, new(0.0f, 1.0f), color);
    }
    void AddLineEnd(int x, int z, Vector3 across, Color color) {
      var pos = GridToWorld(x, z);
      AddVert(pos - across, new(1.0f, 0.0f), color);
      AddVert(pos + across, new(1.0f, 1.0f), color);

      triangles.Add(vertices.Count - 4);
      triangles.Add(vertices.Count - 2);
      triangles.Add(vertices.Count - 3);

      triangles.Add(vertices.Count - 3);
      triangles.Add(vertices.Count - 2);
      triangles.Add(vertices.Count - 1);
    }

    // Vertical lines
    for (int x = 0; x < XPoints; x++) {
      var across = new Vector3(GetLineWidth(x), 0, 0);
      AddLineStart(x, 0, across, GetLineColor(x));
      for (int z = 1; z < ZPoints; z++)
        AddLineEnd(x, z, across, GetLineColor(x));
    }
    // Horizontal lines
    for (int z = 0; z < ZPoints; z++) {
      var across = new Vector3(0, 0, GetLineWidth(z));
      AddLineStart(0, z, across, GetLineColor(z));
      for (int x = 1; x < XPoints; x++)
        AddLineEnd(x, z, across, GetLineColor(z));
    }

    mesh.vertices = Vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.colors = colors.ToArray();
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();
  }

  void UpdateMesh() {
    var points = JobBuffer[BufferIdx];

    // Vertical lines
    int vertIdx = 0;
    for (int x = 0; x < XPoints; x++) {
      var across = new Vector3(GetLineWidth(x), 0, 0);
      for (int z = 0; z < ZPoints; z++) {
        var pi = PointIndex(x, z);
        Vertices[vertIdx++] = points[pi].Position - across;
        Vertices[vertIdx++] = points[pi].Position + across;
      }
    }
    // Horizontal lines
    for (int z = 0; z < ZPoints; z++) {
      var across = new Vector3(0, 0, GetLineWidth(z));
      for (int x = 0; x < XPoints; x++) {
        var pi = PointIndex(x, z);
        Vertices[vertIdx++] = points[pi].Position - across;
        Vertices[vertIdx++] = points[pi].Position + across;
      }
    }
    Mesh.mesh.vertices = Vertices;
  }

  void FixedUpdate() {
    JobHandle.Complete();
    BufferIdx = (BufferIdx+1)%2;
    UpdateMesh();
    GatherForces();
    Job = MakeJob();
    JobHandle = Job.Schedule(JobBuffer[0].Length, 64);
    NumForces = 0;
  }

  void GatherForces() {
    foreach (var f in GameManager.Instance.GridForces) {
      var dir = f.Rigidbody == null ? f.transform.forward : f.Rigidbody.velocity.normalized;
      AddForce(f.transform.position, f.Magnitude, dir, f.Radial, f.Radius, f.ForwardBias);
    }
  }

  void AddForce(Vector3 pos, float magnitude, Vector3 dir, bool isRadial, float radius, float forwardBias = 1f) {
    Forces[NumForces++] = new PForce { Position = pos, Magnitude = magnitude, Direction = dir, Radial = isRadial, Radius = radius, ForwardBias = forwardBias };
  }

  // ----
  // Job junk
  // ----

  [BurstCompile]
  struct PForce {
    public Vector3 Position;
    public Vector3 Direction;
    public bool Radial;
    public float Magnitude;
    public float Radius;
    public float ForwardBias;
  }

  [BurstCompile]
  struct SpringSolverJob : IJobParallelFor {
    [ReadOnly] public NativeArray<SpringData> LastData;
    public NativeArray<SpringData> NextData;
    [ReadOnly] public NativeArray<PForce> Forces;
    public int NumForces;
    public float SpringK;
    public float SpringDamping;
    public float Mass;
    public float DeltaTime;
    public Vector2 GridSpacing;
    public float XMin, ZMin, XMax, ZMax;
    public int XPoints, ZPoints;

    int PointIndex(int x, int z) => z*XPoints + x;
    (int, int) GridIndex(int vi) => (vi % XPoints, vi / XPoints);
    Vector3 GridToWorld(int x, int z) => new(XMin + x*GridSpacing.x, 0f, ZMin + z*GridSpacing.y);
    public Vector3 Clamp(Vector3 v) => new Vector3(Mathf.Clamp(v.x, XMin, XMax), 0, Mathf.Clamp(v.z, ZMin, ZMax));

    SpringData ThisData;
    public void Execute(int i) {
      (var x, var z) = GridIndex(i);
      ThisData = LastData[i];
      ApplyProjectileForces();
      IterateSprings(x, z);
      BoundPosition(x, z);
      NextData[i] = ThisData;
    }

    void BoundPosition(int x, int z) {
      if (x == 0 || z == 0 || x == XPoints-1 || z == ZPoints-1) {
        var p = GridToWorld(x, z);
        ThisData.Position = p;
      } else {
        ThisData.Position = Clamp(ThisData.Position);
      }
    }

    Vector3 NeighborF(Vector3 p, int nx, int nz) {
      if (nx < 0 || nx >= XPoints || nz < 0 || nz >= ZPoints)
        return -SpringK * (p - GridToWorld(nx, nz));  // pretend there's a fixed neighbor for the edge vertices
      var nvi = PointIndex(nx, nz);
      var np = LastData[nvi].Position;
      return -SpringK * (p - np);
    }

    void IterateSprings(int x, int z) {
      var p = ThisData.Position;
      var anchor = GridToWorld(x, z);
      var springF = -SpringK*(p - anchor);
      var dampingF = SpringDamping * ThisData.Velocity;
      var neighborF = NeighborF(p, x-1, z) + NeighborF(p, x+1, z) + NeighborF(p, x, z-1) + NeighborF(p, x, z+1);
      var force = neighborF + springF - dampingF;
      ThisData.Acceleration += force / Mass;
      ThisData.Velocity += DeltaTime * ThisData.Acceleration;
      ThisData.Position += DeltaTime * ThisData.Velocity;
      ThisData.Acceleration = Vector3.zero;
    }

    void ApplyProjectileForces() {
      for (var i = 0; i < NumForces; i++) {
        ApplyForce(Forces[i]);
      }
    }

    void ApplyForce(PForce force) {
      var towardsVert = ThisData.Position - force.Position;
      if (towardsVert.sqrMagnitude > force.Radius.Sqr()) return;

      var distanceFactor = 1.0f - (towardsVert.magnitude / force.Radius);
      Vector3 baseF;
      if (force.Radial) {
        baseF = towardsVert.normalized;
      } else {
        var forward = force.Direction;
        var tangent = Vector3.Cross(forward, Vector3.up);
        var rightSide = Vector3.Dot(towardsVert, tangent) > 0f;
        if (!rightSide) tangent = -tangent;
        baseF = Vector3.Lerp(tangent, forward, force.ForwardBias).normalized;
      }
      ThisData.Acceleration += (force.Magnitude * distanceFactor * baseF) / Mass;
      //gridPoint.m_Damping *= 0.6f;
      //gridPoint.m_LastForceTime = Time.time;
    }

    void ApplyVelocity(PForce force) {
      var vnorm = force.Direction.normalized;
      var tangent = Vector3.Cross(vnorm, Vector3.up);
      var towardsVert = ThisData.Position - force.Position;
      if (towardsVert.sqrMagnitude > force.Radius.Sqr()) return;

      var magnitude = force.Direction.magnitude;
      var rightSide = Vector3.Dot(towardsVert, tangent) > 0f;
      if (!rightSide) tangent = -tangent;
      var baseF = Vector3.Lerp(tangent, vnorm, force.ForwardBias).normalized;
      ThisData.Position += magnitude / towardsVert.magnitude * baseF * DeltaTime;
    }
  }

  SpringSolverJob Job;
  JobHandle JobHandle;
  SpringSolverJob MakeJob() {
    int readIdx = BufferIdx;
    int writeIdx = (BufferIdx+1)%2;
    return new SpringSolverJob() {
      LastData = JobBuffer[readIdx],
      NextData = JobBuffer[writeIdx],
      GridSpacing = GridSpacing,
      XPoints = XPoints,
      ZPoints = ZPoints,
      XMin = Bounds.Instance.XMin,
      ZMin = Bounds.Instance.ZMin,
      XMax = Bounds.Instance.XMax,
      ZMax = Bounds.Instance.ZMax,
      SpringK = SpringK,
      SpringDamping = SpringDamping,
      Mass = Mass,
      Forces = Forces,
      NumForces = NumForces,
      DeltaTime = Time.fixedDeltaTime,
    };
  }

  [ContextMenu("Deform")]
  void Deform() {
    ////var vi = Random.Range(0, Vertices.Length);
    //var vi = Vertices.Length/2;
    //Vertices[vi] += Random.insideUnitSphere.XZ() * 10f;
  }
  //void OnGUI() {
  //  for (int i = 0; i < Mesh.mesh.vertices.Length; i++) {
  //    GUIExtensions.DrawLabel(Mesh.mesh.vertices[i], $"{i}");
  //  }
  //}

}