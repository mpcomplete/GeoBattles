using System.Collections;
using UnityEngine;

public class AnimateSpawn : MonoBehaviour {
  [SerializeField] Transform AnimationSegmentsRoot;
  [SerializeField] Transform SegmentsRoot;
  [SerializeField] Timeval PulseDuration = Timeval.FromTicks(5);
  [SerializeField] float MinLengthScale = 0.5f;
  [SerializeField] float MaxLengthScale = 1.5f;
  [SerializeField] int PulseCount = 3;

  IEnumerator Start() {
    var ticks = PulseDuration.Ticks;
    var localScale = SegmentsRoot.localScale;
    for (var p = 0; p < PulseCount; p++) {
      for (var t = 0; t < ticks; t++) {
        var interpolant = (float)t/(ticks-1);
        var scale = Mathf.Lerp(MinLengthScale, MaxLengthScale, interpolant);
        SegmentsRoot.localScale = scale * Vector3.one;
        yield return new WaitForFixedUpdate();
      }
    }
    SegmentsRoot.localScale = localScale;
  }
}