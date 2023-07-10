using UnityEngine;

public class Bomb : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;
  [SerializeField] float ForceRadius = 15;
  [SerializeField] float ForceScale = 15;

  void Start() {
    InputHandler.OnBomb += TryDetonate;
  }

  void OnDestroy() {
    InputHandler.OnBomb -= TryDetonate;
  }

  void TryDetonate() {
    if (BombManager.Instance.TryDeonateBomb()) {
      Bounds.Instance.VectorGrid.AddGridForce(transform.position, ForceScale, ForceRadius, Color.white, false);
      GameManager.Instance.DespawnMobsSafe(c => true);
    }
  }
}