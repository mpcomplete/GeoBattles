using UnityEngine;

public class BombShockwave : MonoBehaviour {
  [SerializeField] float ExpansionSpeed = 1f;
  [SerializeField] float SegmentLength = 2f;
  [SerializeField] GameObject[] Segments;

  void Start() {
    Segments.ForEach((s, i) => s.transform.eulerAngles = new Vector3(0, i * 360f / Segments.Length, 0));
  }

  float CurrentRadius = 0f;
  void FixedUpdate() {
    CurrentRadius += ExpansionSpeed * Time.fixedDeltaTime;
    Segments.ForEach(s => s.transform.position = transform.position + s.transform.forward*CurrentRadius);
    var outerDistSq = (CurrentRadius + SegmentLength).Sqr();
    var innerDistSq = (CurrentRadius - 2*SegmentLength).Sqr();  // technically don't need SegmentLength, but we'll be generous.
    GameManager.Instance.DespawnMobsSafe(c => {
      var distSq = (c.transform.position - transform.position).sqrMagnitude;
      return distSq >= innerDistSq && distSq <= outerDistSq;
    });
  }
}