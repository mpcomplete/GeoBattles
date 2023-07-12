using UnityEngine;

public class SoftFollowCameraTarget : MonoBehaviour {
  public Vector3 FollowFactor = new(.6f, 0f, .8f);
  public Transform Target { get; private set; }

  void Start() {
    Target = new GameObject().transform;
    Target.position = Vector3.Scale(transform.position, FollowFactor);
  }

  void OnDestroy() {
    if (Target)
      Destroy(Target.gameObject);
  }

  void FixedUpdate() {
    Target.position = Vector3.Scale(transform.position, FollowFactor);  // works because origin is 0,0
  }
}