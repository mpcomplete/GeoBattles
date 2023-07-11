using UnityEngine;

public class GridForce : MonoBehaviour {
  public float Magnitude;
  public float Radius;
  public float ForwardBias;
  public bool Radial = false;
  public Rigidbody Rigidbody;

  void Start() {
    GameManager.Instance.GridForces.Add(this);
  }
  void OnDestroy() {
    GameManager.Instance.GridForces.Remove(this);
  }
}