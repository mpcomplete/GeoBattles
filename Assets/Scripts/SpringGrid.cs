using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SpringGrid : MonoBehaviour {
  public MeshFilter Mesh;
  public Vector2 GridSpacing;
  public float SpringK = 2f;
  public float SpringDamping = 10f;
  public float Mass = 1;

  int XCells, ZCells;
  int XVertices, ZVertices;
  struct Buffer {
    public NativeArray<Vector3> Vertices;
    public NativeArray<Vector3> Velocities;
  }
  Buffer[] JobBuffer = new Buffer[2];
  int BufferIdx = 0;
  NativeArray<PForce> Forces;
  //float[] SpringKLocal;
  int VertexIndex(int x, int z) => z*XVertices + x;
  Vector3 GridToWorld(int x, int z) => new(Bounds.Instance.XMin + x*GridSpacing.x, 0f, Bounds.Instance.ZMin + z*GridSpacing.y);

  [BurstCompile]
  struct PForce {
    public Vector3 pos;
    public Vector3 vel;
    public float mass;
    public float maxDistance;
  }

  [BurstCompile]
  struct SpringSolverJob : IJobParallelFor {
    [ReadOnly] public NativeArray<Vector3> LastVertices;
    [ReadOnly] public NativeArray<Vector3> LastVelocities;
    public NativeArray<Vector3> Vertices;
    public NativeArray<Vector3> Velocities;
    [ReadOnly] public NativeArray<PForce> Forces;
    public int NumForces;
    const float SpringK = 20f;
    const float SpringDamping = 4f;
    const float Mass = 1f;
    public float DeltaTime;

    public Vector2 GridSpacing;
    public int XVertices;
    public int ZVertices;
    int VertexIndex(int x, int z) => z*XVertices + x;
    (int, int) GridIndex(int vi) => (vi % XVertices, vi / XVertices);
    Vector3 GridToWorld(int x, int z) => new(Bounds.Instance.XMin + x*GridSpacing.x, 0f, Bounds.Instance.ZMin + z*GridSpacing.y);

    public void Execute(int i) {
      (var x, var z) = GridIndex(i);
      Vertices[i] = LastVertices[i];
      Velocities[i] = LastVelocities[i];
      IterateSprings(x, z, i);  // must call after
      ApplyProjectileForces(x, z, i);
    }

    Vector3 NeighborF(Vector3 p, int nx, int nz) {
      if (nx < 0 || nx >= XVertices || nz < 0 || nz >= ZVertices)
        return -SpringK * (p - GridToWorld(nx, nz));  // pretend there's a fixed neighbor for the edge vertices
      var nvi = VertexIndex(nx, nz);
      var np = LastVertices[nvi];
      return -SpringK * (p - np);
    }

    void IterateSprings(int x, int z, int vi) {
      var p = Vertices[vi];
      var v = Velocities[vi];
      //var k = SpringKLocal[vi];
      //SpringKLocal[vi] = Mathf.Lerp(SpringKLocal[vi], SpringK, .5f);
      var anchor = GridToWorld(x, z);
      var springF = -SpringK*(p - anchor);
      var dampingF = SpringDamping * v;
      var neighborF = NeighborF(p, x-1, z) + NeighborF(p, x+1, z) + NeighborF(p, x, z-1) + NeighborF(p, x, z+1);
      var force = neighborF + springF - dampingF;
      v += DeltaTime * force / Mass; // mass=1
      p += DeltaTime * v;

      Vertices[vi] = p;
      Velocities[vi] = v;
    }

    const float ProjectileFactor = 1f;
    const float ProjectileRange = 1f;
    const float ProjectileForwardBias = .25f;
    void ApplyProjectileForces(int x, int z, int vi) {
      for (var i = 0; i < NumForces; i++) {
        var f = Forces[i];
        ApplyVelocity(x, z, vi, f.pos, f.vel, f.mass, f.maxDistance);
      }
    }

    void ApplyVelocity(int x, int z, int vi, Vector3 pos, Vector3 vel, float mass, float maxDistance) {
      var vnorm = vel.normalized;
      var tangent = Vector3.Cross(vnorm, Vector3.up);
      var p = Vertices[vi];
      var towardsVert = p - pos;
      if (towardsVert.sqrMagnitude > maxDistance.Sqr()) return;

      var rightSide = Vector3.Dot(towardsVert, tangent) > 0f;
      if (!rightSide) tangent = -tangent;
      var baseF = Vector3.Lerp(tangent, vnorm, ProjectileForwardBias).normalized;
      Velocities[vi] += ProjectileFactor * mass / towardsVert.magnitude * baseF;
      //SpringKLocal[vi] += mass / towardsVert.magnitude;
    }
  }

  SpringSolverJob Job;
  JobHandle JobHandle;

  (int, int) GridIndex(int vi) => (vi % XVertices, vi / XVertices);
  void Start() {
    XCells = (int)(Bounds.Instance.XSize / GridSpacing.x);
    ZCells = (int)(Bounds.Instance.ZSize / GridSpacing.y);
    XVertices = XCells+1;
    ZVertices = ZCells+1;

    int nverts = XVertices * ZVertices;
    JobBuffer[0].Vertices = new NativeArray<Vector3>(nverts, Allocator.Persistent);
    JobBuffer[0].Velocities = new NativeArray<Vector3>(nverts, Allocator.Persistent);
    JobBuffer[1].Vertices = new NativeArray<Vector3>(nverts, Allocator.Persistent);
    JobBuffer[1].Velocities = new NativeArray<Vector3>(nverts, Allocator.Persistent);
    Forces = new NativeArray<PForce>(500, Allocator.Persistent);
    //SpringKLocal = new float[Vertices.Length];
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        var p = GridToWorld(x, z);
        (var tx, var tz) = GridIndex(vi);
        if (!(x == tx && z == tz))
          tx = tx;
        Debug.Assert(x == tx && z == tz, $"Nope: {x}={tx}, {z}={tz}");
        JobBuffer[0].Vertices[vi] = p;
        JobBuffer[0].Velocities[vi] = Vector3.zero;
        JobBuffer[1].Vertices[vi] = p;
        JobBuffer[1].Velocities[vi] = Vector3.zero;
        //SpringKLocal[vi] = SpringK;
      }
    }

    int numCells = XCells * ZCells;
    var triangles = new int[numCells * 2 * 3];
    var ti = 0;
    for (int z = 0; z < ZVertices-1; z++) {
      for (int x = 0; x < XVertices-1; x++) {
        // x,z is the topleft vertex of the cell
        (triangles[ti], triangles[ti+1], triangles[ti+2]) = (VertexIndex(x, z), VertexIndex(x, z+1), VertexIndex(x+1, z));
        ti += 3;
        (triangles[ti], triangles[ti+1], triangles[ti+2]) = (VertexIndex(x+1, z), VertexIndex(x, z+1), VertexIndex(x+1, z+1));
        ti += 3;
      }
    }

    var uvs = new Vector2[nverts];
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        uvs[vi] = new Vector2((float)x / XVertices, (float)z / ZVertices);
      }
    }

    var m = new Mesh();
    m.vertices = JobBuffer[0].Vertices.ToArray();
    m.triangles = triangles;
    m.uv = uvs;
    m.RecalculateNormals();
    Mesh.mesh = m;

    Job = new SpringSolverJob() {
      LastVertices = JobBuffer[0].Vertices,
      LastVelocities = JobBuffer[0].Velocities,
      Vertices = JobBuffer[1].Vertices,
      Velocities = JobBuffer[1].Velocities,
      GridSpacing = GridSpacing,
      XVertices = XVertices,
      ZVertices = ZVertices,
      Forces = Forces,
      NumForces = 0,
      DeltaTime = Time.fixedDeltaTime,
    };
    BufferIdx++;
    JobHandle = Job.Schedule(nverts, 64);
  }

  void FixedUpdate() {
    JobHandle.Complete();
    if (!JobHandle.IsCompleted)
      Debug.Log($"Job: {JobHandle.IsCompleted}");
    Mesh.mesh.vertices = Job.Vertices.ToArray();
    //var tmp = Job.Velocities.ToArray();
    //var vi = 0;
    //foreach (var v in tmp) {
    //  if (v.sqrMagnitude > 0)
    //    Debug.Log($"velocity {vi} = {v}");
    //  vi++;
    //}

    int readIdx = BufferIdx;
    int writeIdx = (BufferIdx+1)%2;
    BufferIdx = writeIdx;
    int numForces = ApplyProjectileForces();
    Job = new SpringSolverJob() {
      LastVertices = JobBuffer[readIdx].Vertices,
      LastVelocities = JobBuffer[readIdx].Velocities,
      Vertices = JobBuffer[writeIdx].Vertices,
      Velocities = JobBuffer[writeIdx].Velocities,
      GridSpacing = GridSpacing,
      XVertices = XVertices,
      ZVertices = ZVertices,
      Forces = Forces,
      NumForces = numForces,
      DeltaTime = Time.fixedDeltaTime,
    };
    JobHandle = Job.Schedule(JobBuffer[0].Vertices.Length, 64);
  }

  public float ProjectileFactor = .05f;
  public float ProjectileRange = 1f;
  public float ProjectileForwardBias = .5f;
  int ApplyProjectileForces() {
    var i = 0;
    foreach (var p in GameManager.Instance.Projectiles) {
      Forces[i++] = new PForce { pos = p.transform.position, vel = p.Rigidbody.velocity, mass = p.Rigidbody.mass, maxDistance = ProjectileRange };
    }
    return i;
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