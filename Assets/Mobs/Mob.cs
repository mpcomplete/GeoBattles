using UnityEngine;

public class Mob : Character {
  public int BaseScore = 0;

  void Start() {
    GlobalOnSpawn = GameManager.Instance.MobSpawn;
    GlobalOnAlive = GameManager.Instance.MobAlive;
    GlobalOnDying = GameManager.Instance.MobDying;
    GlobalOnDeath = GameManager.Instance.MobDeath;
    StartCoroutine(SpawnRoutine());
  }
}