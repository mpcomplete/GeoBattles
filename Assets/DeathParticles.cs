using UnityEngine;

public class DeathParticles : MonoBehaviour {
  [SerializeField] ParticleSystem[] ParticleSystemPrefabs;

  Character Character;

  void Start() {
    Character = GetComponent<Character>();
    Character.OnDying += SpawnParticles;
    Character.OnDespawn += SpawnParticles;
  }

  void OnDestroy() {
    Character.OnDying -= SpawnParticles;
    Character.OnDespawn -= SpawnParticles;
  }

  void SpawnParticles() {
    Instantiate(ParticleSystemPrefabs.Random(), transform.position, transform.rotation);
  }
}