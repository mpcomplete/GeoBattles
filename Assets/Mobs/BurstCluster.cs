using System.Collections;
using UnityEngine;

public class BurstCluster : MonoBehaviour {
  public Transform[] Objects;
  public float BurstForce = 15f;

  void Start() {
    foreach (var o in Objects) {
      o.SetParent(transform.parent, true);
      o.GetComponent<Controller>().AddPhysicsVelocity(BurstForce * (o.position - transform.position).normalized);
    }
    Destroy(gameObject);
  }
}