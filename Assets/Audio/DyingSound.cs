using UnityEngine;

public class DeathSound : MonoBehaviour {
  [SerializeField] AudioSource Source;

  Character Character;

  void Start() {
    Character = GetComponent<Character>();
    Character.OnDying += PlaySound;
    Character.OnDespawn += PlaySound;
  }

  void OnDestroy() {
    Character.OnDying -= PlaySound;
    Character.OnDespawn -= PlaySound;
  }

  void PlaySound() {
    AudioSource.PlayClipAtPoint(Source.clip, transform.position);
  }
}