using UnityEngine;

public class Mob : Character {
  void Start() {
    GlobalOnSpawn = GameManager.Instance.MobSpawn;
    GlobalOnAlive = GameManager.Instance.MobAlive;
    GlobalOnDying = GameManager.Instance.MobDying;
    GlobalOnDeath = GameManager.Instance.MobDeath;
    StartCoroutine(SpawnRoutine());
  }
}