using UnityEngine;

public class Bounds : MonoBehaviour {
  public static Bounds Instance;

  public float XMin;
  public float XMax;
  public float ZMin;
  public float ZMax;

  void Awake() {
    Instance = this;
  }

  void OnDestroy() {
    Instance = null;
  }

  float Bound(float min, float max, float v) {
    return Mathf.Min(Mathf.Max(min, v), max);
  }

  public Vector3 Bound(Vector3 v, float radius) {
    return new Vector3(Bound(XMin+radius, XMax-radius, v.x), 0, Bound(ZMin+radius, ZMax-radius, v.z));
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(Vector3.zero, new Vector3(XMax-XMin, 0, ZMax-ZMin));
  }
}