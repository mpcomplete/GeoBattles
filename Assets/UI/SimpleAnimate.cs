using System.Collections;
using UnityEngine;
using TMPro;

public class SimpleAnimate : MonoBehaviour {
  [SerializeField] Timeval Duration = Timeval.FromSeconds(1);
  [SerializeField] AnimationCurve ScaleCurve = AnimationCurve.Linear(0,1,1,0);
  [SerializeField] AnimationCurve AlphaCurve = AnimationCurve.Linear(0,0,1,1);

  void OnEnable() {
    StartCoroutine(Animate());
  }

  void OnDisable() {
    StopAllCoroutines();
  }

  IEnumerator Animate() {
    var totalTicks = Duration.Ticks;
    for (var i = 0; i < totalTicks; i++) {
      var f = (float)i/totalTicks;
      transform.localScale = ScaleCurve.Evaluate(f) * Vector3.one;
      yield return new WaitForFixedUpdate();
    }
  }
}