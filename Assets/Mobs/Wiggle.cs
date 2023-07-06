using UnityEngine;

public class Wiggle : MonoBehaviour {
  public float WiggleSpeed = 1f;
  public float WiggleMaxDegrees = 45f;
  public AnimationCurve Curve;

  float T;
  void FixedUpdate() {
    T += Time.fixedDeltaTime * WiggleSpeed;
    if (T > 1f) T = 0f;
    transform.localRotation = Quaternion.Euler(0, WiggleMaxDegrees * Curve.Evaluate(T), 0);
  }
}