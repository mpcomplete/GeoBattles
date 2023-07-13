using UnityEngine;

public class Bomb : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;
  [SerializeField] BombShockwave Shockwave;

  void Start() {
    InputHandler.OnBomb += TryDetonate;
  }

  void OnDestroy() {
    InputHandler.OnBomb -= TryDetonate;
  }

  void TryDetonate() {
    if (BombManager.Instance.TryDeonateBomb()) {
      Instantiate(Shockwave, transform.position, Quaternion.identity);
    }
  }
}