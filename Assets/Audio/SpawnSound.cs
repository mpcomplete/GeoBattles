using UnityEngine;

public class SpawnSound : MonoBehaviour {
  [SerializeField] AudioSource Source;

  Character Character;

  void Start() {
    Character = GetComponent<Character>();
    Character.OnSpawn += PlaySound;
  }

  void OnDestroy() {
    Character.OnSpawn -= PlaySound;
  }

  void PlaySound() {
    AudioSource.PlayClipAtPoint(Source.clip, transform.position);
  }
}