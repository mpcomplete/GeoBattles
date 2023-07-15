using UnityEngine;

public class Bomb : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;
  [SerializeField] BombShockwave Shockwave;
  [SerializeField] AudioClip AudioClip;

  void Start() {
    InputHandler.OnBomb += TryDetonate;
  }

  void OnDestroy() {
    InputHandler.OnBomb -= TryDetonate;
  }

  void TryDetonate() {
    if (BombManager.Instance.TryDetonateBomb()) {
      AudioManager.Instance.BombSource.PlayOneShot(AudioClip);
      Instantiate(Shockwave, transform.position, Quaternion.identity);
    }
  }
}