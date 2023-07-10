using UnityEngine;

public static class ProceduralMesh {
  public static Mesh Grid(int columns, int rows, float xmin, float xmax, float zmin, float zmax) {
    int XVertices = columns+1;
    int ZVertices = rows+1;
    float dx = (xmax - xmin)/columns;
    float dz = (zmax - zmin)/rows;

    int VertexIndex(int x, int z) => z*XVertices + x;
    Vector3 GridToWorld(int x, int z) => new(xmin+dx*x, 0f, zmin+dz*z);

    var Vertices = new Vector3[XVertices * ZVertices];
    for (int z = 0; z < ZVertices; z++) {
      for (int x = 0; x < XVertices; x++) {
        var vi = VertexIndex(x, z);
        var p = GridToWorld(x, z);
        Vertices[vi] = p;
      }
    }

    int numCells = columns * rows;
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
    return m;
  }
}