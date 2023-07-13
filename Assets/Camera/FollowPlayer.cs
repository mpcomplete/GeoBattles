using UnityEngine;
using Cinemachine;

public class FollowPlayer : MonoBehaviour {
  [SerializeField] CinemachineVirtualCamera VirtualCamera;

  void Start() {
    GameManager.Instance.PlayerSpawn += Follow;
    GameManager.Instance.PlayerDying += UnFollow;
    GameManager.Instance.PostGame += ReturnToCenter;
  }

  void OnDestroy() {
    GameManager.Instance.PlayerSpawn -= Follow;
    GameManager.Instance.PlayerDying -= UnFollow;
    GameManager.Instance.PostGame -= ReturnToCenter;
  }

  void Follow(Character c) {
    VirtualCamera.Follow = c.GetComponent<SoftFollowCameraTarget>().Target;
  }

  void UnFollow(Character c) {
    VirtualCamera.Follow = null;
  }

  void ReturnToCenter() {
    VirtualCamera.Follow = Bounds.Instance.transform;
  }
}