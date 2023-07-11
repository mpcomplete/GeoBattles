using UnityEngine;

public class DebugToggleShot : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;

  void Start() {
    InputHandler.OnDebugShot += NextShot;
  }

  void OnDestroy() {
    InputHandler.OnDebugShot -= NextShot;
  }

  int NextIdx = 0;
  void NextShot() {
    ShotManager.Instance.SetShotVariant(NextIdx++);
    if (NextIdx >= 3) NextIdx = 0;
  }
}