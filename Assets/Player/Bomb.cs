using UnityEngine;

public class Bomb : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;

  void Start() {
    InputHandler.OnBomb += TryDetonate;
  }

  void OnDestroy() {
    InputHandler.OnBomb -= TryDetonate;
  }

  void TryDetonate() {
    GameManager.Instance.Mobs.ForEach(m => m.Kill());
  }
}
