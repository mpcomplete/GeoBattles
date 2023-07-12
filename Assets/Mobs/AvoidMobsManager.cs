using UnityEngine;

public class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour {
  public static T Instance;

  protected virtual void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this as T;
      DontDestroyOnLoad(gameObject);
      AwakeSingleton();
    }
  }

  protected virtual void AwakeSingleton() { }
}

public class AvoidMobsManager : SingletonBehavior<AvoidMobsManager> {
  public float CellSize = 1f;
  public float SeparationStrength = 1f;
  float HalfCellSize;
  int[,] CellMobCount;
  Vector3[,] CellAverageOffset;
  Vector3[,] CellPressure;

  (int, int) WorldToGrid(Vector3 pos) => ((int)((pos.x - Bounds.Instance.XMin) / CellSize), (int)((pos.z - Bounds.Instance.ZMin) / CellSize));
  Vector3 GridToWorld(int x, int z) => new Vector3(Bounds.Instance.XMin + HalfCellSize + x*CellSize, 0, Bounds.Instance.ZMin + HalfCellSize + z*CellSize);

  public Vector3 GetAvoidForce(Vector3 pos) {
    (var x, var z) = WorldToGrid(pos);

    return SeparationStrength*CellPressure[x, z];
    //return Mathf.Sqrt(CellMobCount[x,z] - 2) * away.normalized;
  }

  public Vector3 GetAvoidForceFail(Vector3 pos) {
    (var x, var z) = WorldToGrid(pos);
    //var away = -CellAverageOffset[x, z];
    //if (away.sqrMagnitude < .01f.Sqr() || CellMobCount[x, z] < 2)
    //  return Vector3.zero;
    //return away.normalized;

    var avgForce = Vector3.zero;
    for (var ix = x-1; ix <= x+1; ix++) {
      if (ix < 0 || ix >= CellMobCount.GetLength(0)) continue;
      for (var iz = z-1; iz <= z+1; iz++) {
        if (iz < 0 || iz >= CellMobCount.GetLength(1)) continue;
        //avgForce -= CellMobCount[ix, iz] * new Vector3((ix-x)/CellSize, 0, (iz-z)/CellSize);
        var center = GridToWorld(ix, iz);
        var avgWorld = center + CellAverageOffset[ix, iz];
        var offset = pos - avgWorld;
        if (offset.sqrMagnitude < .1f.Sqr()) continue;
        avgForce += offset.normalized / offset.sqrMagnitude;
      }
    }
    return SeparationStrength*avgForce;
    //return Mathf.Sqrt(CellMobCount[x,z] - 2) * away.normalized;
  }

  void Start() {
    HalfCellSize = CellSize/2f;
    (var numX, var numZ) = WorldToGrid(new Vector3(Bounds.Instance.XMax, 0, Bounds.Instance.ZMax));
    CellMobCount = new int[numX, numZ];
    CellAverageOffset = new Vector3[numX, numZ];
    CellPressure = new Vector3[numX, numZ];
  }

  private void FixedUpdate() {
    for (var x = 0; x < CellMobCount.GetLength(0); x++) {
      for (var z = 0; z < CellMobCount.GetLength(1); z++) {
        CellMobCount[x, z] = 0;
        CellAverageOffset[x, z] = Vector3.zero;
        CellPressure[x, z] = Vector3.zero;
      }
    }

    foreach (var mob in GameManager.Instance.Mobs) {
      (var x, var z) = WorldToGrid(mob.transform.position);
      var center = GridToWorld(x, z);
      CellMobCount[x, z]++;
      CellAverageOffset[x, z] += (mob.transform.position - center);
    }

    for (var x = 0; x < CellMobCount.GetLength(0); x++) {
      for (var z = 0; z < CellMobCount.GetLength(1); z++) {
        var center = GridToWorld(x, z);
        var localAvg = Vector3.zero;
        for (var ix = x-1; ix <= x+1; ix++) {
          //if (ix < 0 || ix >= CellMobCount.GetLength(0)) continue;
          for (var iz = z-1; iz <= z+1; iz++) {
            //if (iz < 0 || iz >= CellMobCount.GetLength(1)) continue;
            var iworld = GridToWorld(ix, iz);
            if (ix >= 0 && ix < CellMobCount.GetLength(0) && iz >= 0 && iz < CellMobCount.GetLength(1))
              iworld += CellAverageOffset[ix, iz];  // out of bounds cellAvg = 0
            var delta = center - iworld;
            localAvg += delta;
            //if (ix == x && iz == z) {
            //  var center = GridToWorld(x, z);
            //  var pressure = center - CellAverageOffset[ix, iz];
            //  if (pressure.sqrMagnitude > .1f.Sqr())
            //    CellPressure[x, z] += CellMobCount[ix, iz] * pressure.normalized / pressure.magnitude;
            //} else {
            //  var delta = new Vector3((ix-x)/CellSize, 0, (iz-z)/CellSize);
            //  CellPressure[x, z] -= CellMobCount[ix, iz] * delta.normalized / delta.magnitude;
            //}
          }
        }
        CellPressure[x, z] = localAvg;
      }
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