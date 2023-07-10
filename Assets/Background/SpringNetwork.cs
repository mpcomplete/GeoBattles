using System.Collections.Generic;
using UnityEngine;

public class SpringNetwork : MonoBehaviour
{
  public float DAMPING_FACTOR = .1f;
  public float SPRING_FORCE = 1;
  public float STRENGTH = 1;

  public ComputeShader computeShader;
  private ComputeBuffer springBuffer;
  private ComputeBuffer forceBuffer;
  private int updateKernel;
  private List<Vector3> vertices = new List<Vector3>();
  private Mesh mesh;

  void Start()
  {
    updateKernel = computeShader.FindKernel("CSMain");
    mesh = ProceduralMesh.Grid(32, 32, -16, 16, -16, 16);
    GetComponent<MeshFilter>().mesh = mesh;
    mesh.GetVertices(vertices);

    // Create spring buffer
    Spring[] springs = new Spring[vertices.Count];
    for (int i = 0; i < vertices.Count; i++)
    {
      springs[i] = new Spring { currentPosition = vertices[i], restPosition = vertices[i], velocity = Vector3.zero };
    }
    springBuffer = new ComputeBuffer(vertices.Count, Spring.STRIDE);
    springBuffer.SetData(springs);

    // Create force buffer
    Force[] forces = new Force[1] { new Force { position = Vector3.zero, strength = STRENGTH } };
    forceBuffer = new ComputeBuffer(forces.Length, Force.STRIDE);
    forceBuffer.SetData(forces);

    // Assign buffers to shader
    computeShader.SetBuffer(updateKernel, "springBuffer", springBuffer);
    computeShader.SetBuffer(updateKernel, "forceBuffer", forceBuffer);
  }

  void FixedUpdate()
  {
    // var position = Random.onUnitSphere;
    var position = Vector3.zero;
    Force[] forces = new Force[1] { new Force { position = position, strength = STRENGTH } };
    forceBuffer.SetData(forces);

    // Dispatch shader
    computeShader.SetFloat("deltaTime", Time.fixedDeltaTime);
    computeShader.SetFloat("dampingFactor", DAMPING_FACTOR);
    computeShader.SetFloat("springConstant", SPRING_FORCE);
    // computeShader.Dispatch(updateKernel, vertices.Count / 1024 + 1, 1, 1);

    // Retrieve data
    Spring[] springs = new Spring[vertices.Count];
    springBuffer.GetData(springs);

    // Update vertices
    for (int i = 0; i < vertices.Count; i++)
    {
      vertices[i] = springs[i].currentPosition;
    }
    mesh.SetVertices(vertices);
    mesh.RecalculateNormals();
    mesh.UploadMeshData(false);
  }

  void OnDestroy()
  {
    // Release buffers
    springBuffer.Release();
    forceBuffer.Release();
  }
}

public struct Spring
{
  public static int STRIDE = 48;
  public Vector4 restPosition;
  public Vector4 currentPosition;
  public Vector4 velocity;
}

public struct Force
{
  public static int STRIDE = 20;
  public Vector4 position;
  public float strength;
}
