using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class SpringGridShader : MonoBehaviour {
  Material RenderVerticalLines;
  public Material RenderHorizontalLines;
  public Material UpdateSprings;
  public Vector2 GridSpacing;
  public float SpringK = 2f;
  public float SpringDamping = 10f;
  public float Mass = 1;

  int XPoints, ZPoints;
  Texture2D Tex2D;

  struct Buffer {
    public RenderTexture Positions;
    public RenderTexture Velocities;
  }
  Buffer[] Buffers;
  int ReadBuffer = 0;
  int WriteBuffer => (ReadBuffer+1)%2;

  // Force data
  const int MaxForces = 100;
  int NumForces = 0;
  Vector4[] ForcePosition = new Vector4[MaxForces];
  Vector4[] ForceDirection = new Vector4[MaxForces];
  Vector4[] ForceRadialMagnitudeRadiusForwardbias = new Vector4[MaxForces];

  // These 2 must match SpringJob's versions.
  int PointIndex(int x, int z) => z*XPoints + x;
  Vector3 GridToWorld(int x, int z) => new(Bounds.Instance.XMin + x*GridSpacing.x, 0f, Bounds.Instance.ZMin + z*GridSpacing.y);

  RenderTexture MakeTex() {
    var tex = new RenderTexture(XPoints, ZPoints, 0, RenderTextureFormat.ARGBFloat);
    //tex.enableRandomWrite = true;
    tex.Create();
    return tex;
  }

  void Start() {
    RenderVerticalLines = new Material(RenderHorizontalLines);
    XPoints = (int)(Bounds.Instance.XSize / GridSpacing.x) + 1;
    ZPoints = (int)(Bounds.Instance.ZSize / GridSpacing.y) + 1;
    Buffers = new Buffer[2];
    for (var i = 0; i < 2; i++) {
      Buffers[i] = new Buffer {
        Positions = MakeTex(),
        Velocities = MakeTex(),
      };
    }

    // Create a Texture2D to read data from the Render Texture
    Tex2D = new Texture2D(XPoints, ZPoints, TextureFormat.RGBAFloat, false);
    var positions = new NativeArray<float4>(XPoints * ZPoints, Allocator.Temp);
    // Set spring anchor positions.
    for (int z = 0; z < ZPoints; z++) {
      for (int x = 0; x < XPoints; x++) {
        var vi = PointIndex(x, z);
        var p = GridToWorld(x, z);
        positions[vi] = new float4(p, 0);
      }
    }
    Tex2D.SetPixelData(positions, 0);
    Tex2D.Apply();
    Graphics.Blit(Tex2D, Buffers[0].Positions);
    positions.Dispose();
    ReadBuffer = 0;

    UpdateSprings.SetFloat("_SpringK", SpringK);
    UpdateSprings.SetFloat("_SpringDamping", SpringDamping);
    UpdateSprings.SetFloat("_Mass", Mass);
    UpdateSprings.SetVectorArray("_ForcePosition", ForcePosition);
    UpdateSprings.SetVectorArray("_ForceDirection", ForceDirection);
    UpdateSprings.SetVectorArray("_ForceRadialMagnitudeRadiusForwardbias", ForceRadialMagnitudeRadiusForwardbias);
    UpdateSprings.SetInteger("_NumForces", 0);
    UpdateSprings.SetInteger("_XPoints", XPoints);
    UpdateSprings.SetInteger("_ZPoints", ZPoints);
    UpdateSprings.SetVector("_BoundsMin", new Vector4(Bounds.Instance.XMin, 0, Bounds.Instance.ZMin, 0));
    UpdateSprings.SetVector("_BoundsMax", new Vector4(Bounds.Instance.XMax, 0, Bounds.Instance.ZMax, 0));
    UpdateSprings.SetVector("_GridSpacing", new Vector4(GridSpacing.x, GridSpacing.y, 0, 0));

    BuildMesh();
  }

  float GetLineWidth(int idx) => (idx % 4 == 0) ? FatLineHalfWidth : LineHalfWidth;
  Color GetLineColor(int idx) => (idx % 4 == 0) ? FatLineColor : LineColor;
  public float LineHalfWidth = .25f;
  public float FatLineHalfWidth = .4f;
  public Color LineColor = Color.blue;
  public Color FatLineColor = Color.blue;
  Vector3[] Vertices;
  Action BuildMesh(Material material, Vector3 across, Vector2Int dir, int numX, int numZ) {
    var vertices = new List<Vector3>();
    var colors = new List<Color>();
    var uvs = new List<Vector2>();
    var triangles = new List<int>();

    void AddVert(Vector3 pos, Vector2 uv, Color color) {
      vertices.Add(pos);
      uvs.Add(uv);
      colors.Add(color);
    }
    // This could be cleaned up a bunch but it's not worth it.
    across = new Vector3(dir.y, 0, dir.x);
    var forward = new Vector3(dir.x, 0, dir.y);
    void AddLineStart(int x, int z, Vector3 across, Color color) {
      var pos = Vector3.zero;
      AddVert(pos - across, new(0.0f, 0.0f), color);
      AddVert(pos + across, new(0.0f, 1.0f), color);
    }
    void AddLineEnd(int x, int z, Vector3 across, Color color) {
      var pos = forward;
      AddVert(pos - across, new(1.0f, 0.0f), color);
      AddVert(pos + across, new(1.0f, 1.0f), color);

      triangles.Add(vertices.Count - 4);
      triangles.Add(vertices.Count - 2);
      triangles.Add(vertices.Count - 3);

      triangles.Add(vertices.Count - 3);
      triangles.Add(vertices.Count - 2);
      triangles.Add(vertices.Count - 1);
    }
    AddLineStart(0, 0, across, GetLineColor(0));
    AddLineEnd(0, 0, across, GetLineColor(0));

    var mesh = new Mesh();
    mesh.name = this.name + " Line";
    mesh.MarkDynamic();
    mesh.vertices = Vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.uv = uvs.ToArray();
    //mesh.colors = colors.ToArray();
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    material.SetInteger("_XPoints", XPoints);
    material.SetInteger("_ZPoints", ZPoints);
    material.SetFloat("_LineHalfWidth", LineHalfWidth);
    material.SetFloat("_FatLineHalfWidth", FatLineHalfWidth);
    material.SetColor("_Color", LineColor);
    material.SetColor("_FatColor", FatLineColor);
    material.SetVector("_BoundsMin", new Vector4(Bounds.Instance.XMin, 0, Bounds.Instance.ZMin, 0));
    material.SetVector("_BoundsMax", new Vector4(Bounds.Instance.XMax, 0, Bounds.Instance.ZMax, 0));
    material.SetVector("_GridSpacing", new Vector4(GridSpacing.x, GridSpacing.y, 0, 0));
    material.SetVector("_Dir", new Vector4(dir.x, 0, dir.y, 0));
    RenderParams rp = new RenderParams(material);
    rp.matProps = new MaterialPropertyBlock();
    var numInstances = numX * numZ;
    var instanceData = new Matrix4x4[numInstances];
    var startIndex = new float[numInstances];
    var endIndex = new float[numInstances];

    var idx = 0;
    for (int z = 0; z < numZ; z++) {
      for (int x = 0; x < numX; x++) {
        instanceData[idx] = Matrix4x4.identity;
        startIndex[idx] = PointIndex(x, z);
        endIndex[idx] = PointIndex(x+dir.x, z+dir.y);
        idx++;
      }
    }

    rp.matProps.SetFloatArray("_StartIndex", startIndex);
    rp.matProps.SetFloatArray("_EndIndex", endIndex);
    return () => {
      Graphics.RenderMeshInstanced(rp, mesh, 0, instanceData, numInstances, 0);
    };
  }
  Action RenderVerticalLinesFunc;
  Action RenderHorizontalLinesFunc;

  void BuildMesh() {
    RenderVerticalLinesFunc = BuildMesh(RenderVerticalLines, new Vector3(GetLineWidth(0), 0, 0), new(0, 1), XPoints, ZPoints-1);
    RenderHorizontalLinesFunc = BuildMesh(RenderHorizontalLines, new Vector3(0, 0, GetLineWidth(0)), new(1, 0), XPoints-1, ZPoints);
  }

  private void Update() {
    GatherForces();

    UpdateSprings.SetTexture("_Positions", Buffers[ReadBuffer].Positions);
    UpdateSprings.SetTexture("_Velocities", Buffers[ReadBuffer].Velocities);
    UpdateSprings.SetVectorArray("_ForcePosition", ForcePosition);
    UpdateSprings.SetVectorArray("_ForceDirection", ForceDirection);
    UpdateSprings.SetVectorArray("_ForceRadialMagnitudeRadiusForwardbias", ForceRadialMagnitudeRadiusForwardbias);
    UpdateSprings.SetInteger("_NumForces", NumForces);
    UpdateSprings.SetFloat("_DeltaTime", Time.deltaTime);
    MultiTargetBlit(Buffers[WriteBuffer], UpdateSprings, 0);
    ReadBuffer = WriteBuffer;

    RenderVerticalLines.SetTexture("_Positions", Buffers[ReadBuffer].Positions);
    RenderHorizontalLines.SetTexture("_Positions", Buffers[ReadBuffer].Positions);
    RenderVerticalLinesFunc();
    RenderHorizontalLinesFunc();
  }

  void GatherForces() {
    NumForces = 0;
    foreach (var f in GameManager.Instance.GridForces) {
      if (NumForces >= ForcePosition.Length)
        break;
      var dir = f.Rigidbody == null ? f.transform.forward : f.Rigidbody.velocity.normalized;
      ForcePosition[NumForces] = f.transform.position;
      ForceDirection[NumForces] = dir;
      ForceRadialMagnitudeRadiusForwardbias[NumForces] = new Vector4(f.Radial ? 1f : 0f, f.Magnitude, f.Radius, f.ForwardBias);
      NumForces++;
    }
  }

  static void MultiTargetBlit(Buffer b, Material mat, int pass = 0) {
    RenderBuffer[] rb = new RenderBuffer[2];
    rb[0] = b.Positions.colorBuffer;
    rb[1] = b.Velocities.colorBuffer;

    //Set the targets to render into.
    //Will use the depth buffer of the
    //first render texture provided.
    Graphics.SetRenderTarget(rb, b.Positions.depthBuffer);

    GL.Clear(true, true, Color.clear);

    GL.PushMatrix();
    GL.LoadOrtho();

    mat.SetPass(pass);

    GL.Begin(GL.QUADS);
    GL.TexCoord2(0, 0); GL.Vertex3(0, 0, 1);
    GL.TexCoord2(1, 0); GL.Vertex3(1, 0, 1);
    GL.TexCoord2(1, 1); GL.Vertex3(1, 1, 1);
    GL.TexCoord2(0, 1); GL.Vertex3(0, 1, 1);
    GL.End();

    GL.PopMatrix();
  }
}
