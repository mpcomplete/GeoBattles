using UnityEngine;

public class Controller : MonoBehaviour {
  public float Radius = .5f;

  public void Move(Vector3 deltaV) {
    transform.position = Bounds.Instance.Bound(transform.position+deltaV, Radius);
  }

  public void Rotation(Quaternion rotation) {
    transform.rotation = rotation;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(transform.position, Radius);
  }
}