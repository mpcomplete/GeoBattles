using System;
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

  static int NumParticles = 0;
  const int MaxParticles = 20;
  void SpawnParticles() {
    if (NumParticles >= MaxParticles)
      return;
    NumParticles++;
    var obj = Instantiate(ParticleSystemPrefabs.Random(), transform.position, transform.rotation);
    obj.GetComponent<ObjectTracker>().OnDestroyed += ParticlesDestroyed;
  }

  void ParticlesDestroyed() {
    NumParticles--;
  }
}