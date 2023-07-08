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
    if (BombManager.Instance.TryDeonateBomb()) {
      GameManager.Instance.DespawnMobsSafe(c => true);
    }
  }
}