using UnityEngine;

public class ShatterSegments : MonoBehaviour {
  [SerializeField] float ExplosionForce = 5;
  [SerializeField] Character Character;
  [SerializeField] GameObject SegmentsRoot;

  void Start() {
    Character.OnDying += Shatter;
    Character.OnDespawn += Shatter;
  }

  void OnDestroy() {
    Character.OnDying -= Shatter;
    Character.OnDespawn -= Shatter;
  }

  void Shatter() {
    foreach (var segment in SegmentsRoot.GetComponentsInChildren<Segment>()) {
      segment.Break(ExplosionForce, transform.position);
    }
  }
}