using UnityEngine;

public class SpringGrid : MonoBehaviour {
  public MeshFilter Mesh;
  public Vector2 GridSpacing;
  public float SpringK = 2f;
  public float SpringDamping = 10f;
  public float Mass = 1;

  int XCells, ZCells;
  int XVertices, ZVertices;
  Vector3[] Vertices;
  Vector3[] Velocities;
  int VertexIndex(int x, int z) => z*XVertices + x;
  Vector3 GridToWorld(int x, int z) => new(Bounds.Instance.XMin + x*GridSpacing.x, 0f, Bounds.Instance.ZMin + z*GridSpacing.y);

  void Start() {
    XCells = (int)(Bounds.Instance.XSize / GridSpacing.x);
    ZCells = (int)(Bounds.Instance.ZSize / GridSpacing.y);
    XVertices = XCells+1;
    ZVertices = ZCells+1;

    Vertices = new Vector3[XVertices * ZVertices];
    Velocities = new Vector3[Vertices.Length];
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        var p = GridToWorld(x, z);
        Vertices[vi] = p;
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

    var uvs = new Vector2[Vertices.Length];
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        uvs[vi] = new Vector2((float)x / XVertices, (float)z / ZVertices);
      }
    }

    var m = new Mesh();
    m.vertices = Vertices;
    m.triangles = triangles;
    m.uv = uvs;
    m.RecalculateNormals();
    Mesh.mesh = m;
  }

  void FixedUpdate() {
    ApplyProjectileForces();
    IterateSprings();
  }

  void IterateSprings() {
      // TODO: Is this "technically correct" thing actually necessary?
      var nextV = new Vector3[Velocities.Length];
    var nextP = new Vector3[Vertices.Length];
    //var nextV = Velocities;
    //var nextP = Vertices;
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        var p = Vertices[vi];
        var v = Velocities[vi];
        var anchor = GridToWorld(x, z);
        var springF = -SpringK*(p - anchor);
        var dampingF = SpringDamping * v;
        Vector3 NeighborF(int nx, int nz) {
          if (nx < 0 || nx >= XVertices || nz < 0 || nz >= ZVertices)
            return -SpringK * (p - GridToWorld(nx, nz));  // pretend there's a fixed neighbor for the edge vertices
          var nvi = VertexIndex(nx, nz);
          var np = Vertices[nvi];
          return -SpringK * (p - np);
        }
        var neighborF = NeighborF(x-1, z) + NeighborF(x+1, z) + NeighborF(x, z-1) + NeighborF(x, z+1);
        var force = neighborF + springF - dampingF;
        v += Time.fixedDeltaTime * force / Mass; // mass=1
        p += Time.fixedDeltaTime * v;
        //if ((p-anchor).sqrMagnitude > 0.1f.Sqr()) {
        //  Debug.Log($"Point {p}, force={force}, vel={v}");
        //}
        nextV[vi] = v;
        nextP[vi] = p;
      }
    }
    Velocities = nextV;
    Vertices = nextP;
    Mesh.mesh.vertices = Vertices;
  }

  public float ProjectileFactor = .05f;
  public float ProjectileRange = 1f;
  public float ProjectileForwardBias = .5f;
  void ApplyProjectileForces() {
    foreach (var p in GameManager.Instance.Projectiles) {
      ApplyVelocity(p.transform.position, p.Rigidbody.velocity, ProjectileRange);
    }
  }

  void ApplyVelocity(Vector3 pos, Vector3 vel, float maxDistance) {
    var vnorm = vel.normalized;
    var tangent = Vector3.Cross(vnorm, Vector3.up);
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        var p = Vertices[vi];
        var towardsVert = p - pos;
        if (towardsVert.sqrMagnitude > maxDistance.Sqr()) continue;

        var rightSide = Vector3.Dot(towardsVert, tangent) > 0f;
        if (!rightSide) tangent = -tangent;
        var baseF = Vector3.Lerp(tangent, vnorm, ProjectileForwardBias).normalized;
        Velocities[vi] += baseF * towardsVert.magnitude * ProjectileFactor;
      }
    }
  }

  [ContextMenu("Deform")]
  void Deform() {
    //var vi = Random.Range(0, Vertices.Length);
    var vi = Vertices.Length/2;
    Vertices[vi] += Random.insideUnitSphere.XZ() * 10f;
  }
  //void OnGUI() {
  //  for (int i = 0; i < Mesh.mesh.vertices.Length; i++) {
  //    GUIExtensions.DrawLabel(Mesh.mesh.vertices[i], $"{i}");
  //  }
  //}
}