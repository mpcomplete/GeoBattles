using UnityEngine;

public class AvoidMobsManager : LevelManager<AvoidMobsManager> {
  public float CellSize = 1f;
  public float SeparationStrength = 1f;
  float HalfCellSize;
  int[,] CellMobCount;
  float[,] CellMobFactor;
  Vector3[,] CellAverageOffset;

  (int, int) WorldToGrid(Vector3 pos) => ((int)((pos.x - Bounds.Instance.XMin) / CellSize), (int)((pos.z - Bounds.Instance.ZMin) / CellSize));
  Vector3 GridToWorld(int x, int z) => new Vector3(Bounds.Instance.XMin + HalfCellSize + x*CellSize, 0, Bounds.Instance.ZMin + HalfCellSize + z*CellSize);

  void Start() {
    HalfCellSize = CellSize/2f;
    (var numX, var numZ) = WorldToGrid(new Vector3(Bounds.Instance.XMax, 0, Bounds.Instance.ZMax));
    CellMobCount = new int[numX, numZ];
    CellMobFactor = new float[numX, numZ];
    CellAverageOffset = new Vector3[numX, numZ];
  }

  private void FixedUpdate() {
    var numX = CellMobCount.GetLength(0);
    var numZ = CellMobCount.GetLength(1);
    for (var x = 0; x < numX; x++) {
      for (var z = 0; z < numZ; z++) {
        CellMobCount[x, z] = 0;
        CellAverageOffset[x, z] = Vector3.zero;
      }
    }

    foreach (var mob in GameManager.Instance.Mobs) {
      (var x, var z) = WorldToGrid(mob.transform.position);
      var center = GridToWorld(x, z);
      CellMobCount[x, z]++;
      CellAverageOffset[x, z] += (mob.transform.position - center);
    }

    for (var x = 0; x < numX; x++) {
      for (var z = 0; z < numZ; z++) {
        CellMobFactor[x, z] = CellMobCount[x, z] > 0f ? Mathf.Sqrt(CellMobCount[x, z]) : 0f;
      }
    }

    foreach (var mob in GameManager.Instance.Mobs) {
      (var x, var z) = WorldToGrid(mob.transform.position);
      if (CellMobCount[x, z] < 2) continue;
      var center = GridToWorld(x, z);
      var worldOffset = center + CellAverageOffset[x, z];
      var outward = mob.transform.position - worldOffset;
      mob.GetComponent<Controller>().AddPhysicsVelocity(Time.fixedDeltaTime * CellMobFactor[x, z] * SeparationStrength * outward.normalized);
    }
  }

  //void OnGUI() {
  //  for (var x = 0; x < CellMobCount.GetLength(0); x++) {
  //    for (var z = 0; z < CellMobCount.GetLength(1); z++) {
  //      var center = GridToWorld(x, z);
  //      GUIExtensions.DrawLabel(center, $"{CellMobCount[x, z]}");
  //      GUIExtensions.DrawLine(center, center + CellPressure[x, z], 1);
  //    }
  //  }
  //}
  }