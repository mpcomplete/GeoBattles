using UnityEngine;

public class Bomb : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;
  [SerializeField] BombShockwave Shockwave;
  [SerializeField] AudioSource AudioSource;

  void Start() {
    InputHandler.OnBomb += TryDetonate;
  }

  void OnDestroy() {
    InputHandler.OnBomb -= TryDetonate;
  }

  void TryDetonate() {
    if (BombManager.Instance.TryDetonateBomb()) {
      AudioSource.PlayOneShot(AudioSource.clip);
      Instantiate(Shockwave, transform.position, Quaternion.identity);
    }
  }
}