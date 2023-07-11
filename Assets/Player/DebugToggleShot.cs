using UnityEngine;

public class DebugToggleShot : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;

  void Start() {
    InputHandler.OnBomb += NextShot;
  }

  void OnDestroy() {
    InputHandler.OnBomb -= NextShot;
  }

  int NextIdx = 0;
  void NextShot() {
    ShotManager.Instance.SetShotVariant(NextIdx++);
    if (NextIdx >= 3) NextIdx = 0;
  }
}