using UnityEngine;

public class Controller : MonoBehaviour {
  public float Radius = .5f;

  public void Move(Vector3 deltaV) {
    var hit = new BoundHit();
    var didHit = Bounds.Instance.Collide(this, deltaV, ref hit);
    if (didHit) {
      SendMessage("OnBoundsHit", hit);
      Debug.DrawRay(hit.Point, hit.Normal);
    }
  }

  public void Rotation(Quaternion rotation) {
    transform.rotation = rotation;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, Radius);
  }
}