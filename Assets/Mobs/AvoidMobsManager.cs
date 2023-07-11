using UnityEngine;

public class SingletonBehavior : MonoBehaviour {
  public static SingletonBehavior SingletonInstance;

  void Awake() {
    if (SingletonInstance) {
      Destroy(gameObject);
    } else {
      SingletonInstance = this;
      DontDestroyOnLoad(gameObject);
      AwakeSingleton();
    }
  }

  protected virtual void AwakeSingleton() { }
}

public class AvoidMobsManager : SingletonBehavior {
  public static AvoidMobsManager Instance => (AvoidMobsManager)SingletonInstance;
  public float CellSize = 1f;
  float HalfCellSize;
  int[,] MobsInCell;

  (int, int) WorldToGrid(Vector3 pos) => ((int)((pos.x - Bounds.Instance.XMin) / CellSize), (int)((pos.z - Bounds.Instance.ZMin) / CellSize));
  Vector3 GridToWorld(int x, int z) => new Vector3(Bounds.Instance.XMin + HalfCellSize + x*CellSize, 0, Bounds.Instance.ZMin + HalfCellSize + z*CellSize);

  public Vector3 GetAvoidForce(Vector3 pos) {
    (var x, var z) = WorldToGrid(pos);
    var center = GridToWorld(x, z);
    var away = pos - center;
    if (away.sqrMagnitude < .01f.Sqr())
      away = Vector3.right;
    return Mathf.Max(0f, MobsInCell[x, z]-3) * away.normalized;
  }

  void Start() {
    HalfCellSize = CellSize/2f;
    (var numX, var numZ) = WorldToGrid(new Vector3(Bounds.Instance.XMax, 0, Bounds.Instance.ZMax));
    MobsInCell = new int[numX, numZ];
  }

  private void FixedUpdate() {
    for (var x = 0; x < MobsInCell.GetLength(0); x++) {
      for (var z = 0; z < MobsInCell.GetLength(1); z++) {
        MobsInCell[x, z] = 0;
      }
    }

    foreach (var mob in GameManager.Instance.Mobs) {
      (var x, var z) = WorldToGrid(mob.transform.position);
      MobsInCell[x, z]++;
    }
  }

  void OnGUI() {
    for (var x = 0; x < MobsInCell.GetLength(0); x++) {
      for (var z = 0; z < MobsInCell.GetLength(1); z++) {
        GUIExtensions.DrawLabel(GridToWorld(x, z), $"{MobsInCell[x, z]}");
      }
    }
  }
}