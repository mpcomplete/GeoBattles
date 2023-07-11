using UnityEngine;

public class DamageParticles : MonoBehaviour {
  [SerializeField] Character Character;
  [SerializeField] ParticleSystem[] ParticleSystemPrefabs;

  void Start() {
    Character.OnDamage += SpawnParticles;
  }

  void OnDestroy() {
    Character.OnDamage -= SpawnParticles;
  }

  void SpawnParticles() {
    Instantiate(ParticleSystemPrefabs.Random(), transform.position, transform.rotation);
  }
}