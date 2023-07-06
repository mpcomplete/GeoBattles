using System;
using UnityEngine;

[Serializable]
public struct BoundHit {
  public Vector3 Point;
  public Vector3 Normal;
}

public class Bounds : MonoBehaviour {
  public static Bounds Instance;

  public float XMin;
  public float XMax;
  public float ZMin;
  public float ZMax;
  public float XSize => XMax - XMin;
  public float ZSize => ZMax - ZMin;

  public Transform NorthPlane;
  public Transform SouthPlane;
  public Transform WestPlane;
  public Transform EastPlane;

  void Awake() {
    Instance = this;
  }

  void OnDestroy() {
    Instance = null;
  }

  float Bound(float min, float max, float v) {
    return Mathf.Min(Mathf.Max(min, v), max);
  }

  Vector3 Bound(Vector3 v, float radius) {
    return new Vector3(Bound(XMin+radius, XMax-radius, v.x), 0, Bound(ZMin+radius, ZMax-radius, v.z));
  }

  public bool Collide(Controller c, Vector3 dp, ref BoundHit hit) {
    var p0 = c.transform.position;
    var p1 = p0 + dp;

    // Check each boundary separately
    bool hitXMin = p1.x - c.Radius < XMin;
    bool hitXMax = p1.x + c.Radius > XMax;
    bool hitZMin = p1.z - c.Radius < ZMin;
    bool hitZMax = p1.z + c.Radius > ZMax;

    var boundP1 = Bound(p1, c.Radius);
    c.transform.position = boundP1;

    // No collision if no boundaries were hit
    if (!(hitXMin || hitXMax || hitZMin || hitZMax)) {
      return false;
    }

    // Handle each collision case
    if (hitXMin) {
      hit.Point = new Vector3(XMin, 0, p1.z);
      hit.Normal = new Vector3(1, 0, 0);
    } else if (hitXMax) {
      hit.Point = new Vector3(XMax, 0, p1.z);
      hit.Normal = new Vector3(-1, 0, 0);
    } else if (hitZMin) {
      hit.Point = new Vector3(p1.x, 0, ZMin);
      hit.Normal = new Vector3(0, 0, 1);
    } else if (hitZMax) {
      hit.Point = new Vector3(p1.x, 0, ZMax);
      hit.Normal = new Vector3(0, 0, -1);
    }

    // Handle corner cases
    if (hitXMin && hitZMin) {
      if (Vector3.Distance(p1, new Vector3(XMin, 0, ZMin)) < c.Radius) {
        hit.Point = new Vector3(XMin, 0, ZMin);
        hit.Normal = (hit.Point - p1).normalized;
      }
    } else if (hitXMin && hitZMax) {
      if (Vector3.Distance(p1, new Vector3(XMin, 0, ZMax)) < c.Radius) {
        hit.Point = new Vector3(XMin, 0, ZMax);
        hit.Normal = (hit.Point - p1).normalized;
      }
    } else if (hitXMax && hitZMin) {
      if (Vector3.Distance(p1, new Vector3(XMax, 0, ZMin)) < c.Radius) {
        hit.Point = new Vector3(XMax, 0, ZMin);
        hit.Normal = (hit.Point - p1).normalized;
      }
    } else if (hitXMax && hitZMax) {
      if (Vector3.Distance(p1, new Vector3(XMax, 0, ZMax)) < c.Radius) {
        hit.Point = new Vector3(XMax, 0, ZMax);
        hit.Normal = (hit.Point - p1).normalized;
      }
    }
    return true;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(Vector3.zero, new Vector3(XMax-XMin, 0, ZMax-ZMin));
  }
}