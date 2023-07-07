using UnityEngine;

public class BurstCluster : MonoBehaviour {
  public Transform[] Objects;
  public float BurstForce = 0f;

  void Start() {
    foreach (var t in Objects) {
      t.SetParent(null, true);
      t.GetComponent<Controller>().AddPhysicsAccel(BurstForce / Time.fixedDeltaTime * (t.position - transform.position).normalized);
    }
    Destroy(gameObject);
  }
}