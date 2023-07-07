using UnityEngine;

public class Mob : Character {
  public int BaseScore = 0;

  // TODO: BlackHoles are special.
  public int Score => BaseScore;

  void Start() {
    GlobalOnSpawn = GameManager.Instance.MobSpawn;
    GlobalOnAlive = GameManager.Instance.MobAlive;
    GlobalOnDying = GameManager.Instance.MobDying;
    GlobalOnDeath = GameManager.Instance.MobDeath;
    Spawn();
  }

  void OnDestroy() {
    GameManager.Instance.Mobs.Remove(this); // black hole might've eaten us.
  }
}