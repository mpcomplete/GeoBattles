using UnityEngine;

// Used for spawning a cluster of objects. Deletes self immediately.
public class Cluster : MonoBehaviour {
  void Start() {
    while (transform.childCount > 0)
      transform.GetChild(0).SetParent(transform.parent, true);
    Destroy(gameObject);
  }
}