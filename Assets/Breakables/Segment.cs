using System.Collections;
using UnityEngine;

public class Segment : MonoBehaviour {
  [SerializeField] Timeval Lifetime = Timeval.FromSeconds(1);
  [SerializeField] Rigidbody Rigidbody;

  Vector3 InitialLocalScale;

  void Awake() {
    InitialLocalScale = transform.localScale;
  }

  public void Break(float explosionForce, Vector3 explosionOrigin) {
    Rigidbody.isKinematic = false;
    Rigidbody.AddExplosionForce(Random.Range(0, explosionForce), explosionOrigin, 1, 0, ForceMode.Impulse);
    Rigidbody.AddTorque(0, Random.Range(-explosionForce, explosionForce), 0, ForceMode.Impulse);
    transform.SetParent(null, true);
    StartCoroutine(Shrink());
  }

  IEnumerator Shrink() {
    for (var i = 0; i < Lifetime.Ticks; i++) {
      transform.localScale = Vector3.Lerp(Vector3.zero, InitialLocalScale, (1f-(float)i/Lifetime.Ticks));
      yield return new WaitForFixedUpdate();
    }
    Destroy(gameObject);
  }
}