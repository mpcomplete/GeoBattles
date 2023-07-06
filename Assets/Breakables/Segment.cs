using System.Collections;
using UnityEngine;

public class Segment : MonoBehaviour {
  [SerializeField] Timeval Lifetime = Timeval.FromSeconds(1);
  [SerializeField] Rigidbody Rigidbody;

  public void Break(float explosionForce, Vector3 explosionOrigin) {
    Rigidbody.isKinematic = false;
    Rigidbody.AddExplosionForce(Random.Range(0, explosionForce), explosionOrigin, 1, 0, ForceMode.Impulse);
    Rigidbody.AddTorque(0, Random.Range(-explosionForce, explosionForce), 0, ForceMode.Impulse);
    transform.SetParent(null, true);
    StartCoroutine(Shrink());
  }

  IEnumerator Shrink() {
    for (var i = 0; i < Lifetime.Ticks; i++) {
      transform.localScale = (1f-(float)i/Lifetime.Ticks) * Vector3.one;
      yield return new WaitForFixedUpdate();
    }
    Destroy(gameObject);
  }
}