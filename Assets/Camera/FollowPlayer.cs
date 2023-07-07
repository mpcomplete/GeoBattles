using UnityEngine;
using Cinemachine;

public class FollowPlayer : MonoBehaviour {
  [SerializeField] CinemachineVirtualCamera VirtualCamera;

  void Start() {
    GameManager.Instance.PlayerSpawn += Follow;
    GameManager.Instance.PlayerDeath += UnFollow;
  }

  void OnDestroy() {
    GameManager.Instance.PlayerSpawn -= Follow;
    GameManager.Instance.PlayerDeath -= UnFollow;
  }

  void Follow(Character c) {
    VirtualCamera.Follow = c.transform;
  }

  void UnFollow(Character c) {
    VirtualCamera.Follow = null;
  }
}
