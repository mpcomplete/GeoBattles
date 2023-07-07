using UnityEngine;

public class DeathParticles : MonoBehaviour {
  [SerializeField] Character Character;
  [SerializeField] ParticleSystem[] ParticleSystemPrefabs;

  void Start() {
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