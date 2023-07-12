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

  (int, int) WorldToGrid(Vector3 pos) => ((int)((pos.x - Bounds.Instance.XMin) / CellSize), (int)((pos.z - Bounds.Instance.ZMin) / CellSize));
  Vector3 GridToWorld(int x, int z) => new Vector3(Bounds.Instance.XMin + HalfCellSize + x*CellSize, 0, Bounds.Instance.ZMin + HalfCellSize + z*CellSize);

  public Vector3 GetAvoidForce(Vector3 pos) {
    (var x, var z) = WorldToGrid(pos);
    var away = -CellAverageOffset[x, z];
    if (away.sqrMagnitude < .01f.Sqr() || CellMobCount[x, z] < 2)
      return Vector3.zero;
    return away.normalized;
    //return Mathf.Sqrt(CellMobCount[x,z] - 2) * away.normalized;
  }

  void Start() {
    HalfCellSize = CellSize/2f;
    (var numX, var numZ) = WorldToGrid(new Vector3(Bounds.Instance.XMax, 0, Bounds.Instance.ZMax));
    CellMobCount = new int[numX, numZ];
    CellAverageOffset = new Vector3[numX, numZ];
  }

  private void FixedUpdate() {
    for (var x = 0; x < CellMobCount.GetLength(0); x++) {
      for (var z = 0; z < CellMobCount.GetLength(1); z++) {
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
  }

  //void OnGUI() {
  //  for (var x = 0; x < CellMobCount.GetLength(0); x++) {
  //    for (var z = 0; z < CellMobCount.GetLength(1); z++) {
  //    var center = GridToWorld(x, z);
  //      GUIExtensions.DrawLabel(center, $"{CellMobCount[x, z]}");
  //      GUIExtensions.DrawLine(center, center + CellAverageOffset[x, z], 1);
  //    }
  //  }
  //}
}