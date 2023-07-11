using System.Collections;
using UnityEngine;

public class Segment : MonoBehaviour {
  [SerializeField] Timeval Lifetime = Timeval.FromSeconds(1);
  Vector3 InitialLocalScale;

  void Awake() {
    InitialLocalScale = transform.localScale;
  }

  public void Break(float explosionForce, Vector3 explosionOrigin) {
    var force = Random.Range(0, explosionForce) * (transform.position - explosionOrigin).normalized;
    var torque = Random.Range(-explosionForce, explosionForce);
    transform.SetParent(null, true);
    StartCoroutine(ShrinkAndFling(force, torque));
  }

  const float Damping = 0.97f;
  IEnumerator ShrinkAndFling(Vector3 force, float torque) {
    var rotation = Quaternion.Euler(0f, torque, 0f);
    for (var i = 0; i < Lifetime.Ticks; i++) {
      transform.localPosition += Time.fixedDeltaTime * force;
      transform.localRotation = rotation * transform.localRotation;
      transform.localScale = Vector3.Lerp(Vector3.zero, InitialLocalScale, (1f-(float)i/Lifetime.Ticks));
      force *= Damping;
      torque *= Damping;
      yield return new WaitForFixedUpdate();
    }
    Destroy(gameObject);
  }
}