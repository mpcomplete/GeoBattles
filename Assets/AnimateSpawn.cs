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
    var segmentCount = SegmentsRoot.childCount;
    var segments = new GameObject[PulseCount][];
    var localScales = new Vector3[PulseCount][];
    var localPositions = new Vector3[PulseCount][];
    // deactivate the segments
    for (var s = 0; s < segmentCount; s++) {
      SegmentsRoot.GetChild(s).gameObject.SetActive(false);
    }
    // populate the segment arrays and localscales
    for (var p = 0; p < PulseCount; p++) {
      segments[p] = new GameObject[segmentCount];
      localScales[p] = new Vector3[segmentCount];
      localPositions[p] = new Vector3[segmentCount];
    }
    // populate the segment instances and localScales
    for (var s = 0; s < segmentCount; s++) {
      var segment = SegmentsRoot.GetChild(s).gameObject;
      for (var p = 0; p < PulseCount; p++) {
        // TODO: Could remove rigidbody and behavior scripts?
        segments[p][s] = Instantiate(segment, AnimationSegmentsRoot);
        localScales[p][s] = segment.transform.localScale;
        localPositions[p][s] = segment.transform.localPosition;
      }
    }
    // cycle through the pulses animating properties like it's 1992
    for (var p = 0; p < PulseCount; p++) {
      // activate this pulses segments
      for (var s = 0; s < segmentCount; s++) {
        segments[p][s].SetActive(true);
      }
      for (var t = 0; t < ticks; t++) {
        for (var s = 0; s < segmentCount; s++) {
          var interpolant = (float)t/(ticks-1);
          var localScale = localScales[p][s];
          var length = Mathf.Lerp(MinLengthScale, MaxLengthScale, interpolant);
          segments[p][s].transform.localScale = new Vector3(localScale.x, localScale.y, length);
          var localPosition = localPositions[p][s];
          segments[p][s].transform.localPosition = length * localPosition;
        }
        yield return new WaitForFixedUpdate();
      }
      // deactivate this pulses segments
      for (var s = 0; s < segmentCount; s++) {
        segments[p][s].SetActive(false);
      }
    }
    // Destroy all the temporary gameobjects
    for (var p = 0; p < PulseCount; p++) {
      for (var s = 0; s < segmentCount; s++) {
        Destroy(segments[p][s]);
      }
    }
    // activate the segments
    for (var s = 0; s < segmentCount; s++) {
      SegmentsRoot.GetChild(s).gameObject.SetActive(true);
    }
  }
}