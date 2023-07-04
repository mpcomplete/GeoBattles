using UnityEngine;

public class AnimateDiamond : MonoBehaviour {
  public float BreatheRate = 1f;
  public AnimationCurve Curve;

  float T;
  void FixedUpdate() {
    T += Time.fixedDeltaTime * BreatheRate;
    if (T > 1f) T = 0f;
    transform.localScale = new(1, 1, Curve.Evaluate(T));
  }
}