using UnityEngine;

public class Player : Character {
  void Start() {
    GlobalOnSpawn = GameManager.Instance.PlayerSpawn;
    GlobalOnAlive = GameManager.Instance.PlayerAlive;
    GlobalOnDying = GameManager.Instance.PlayerDying;
    GlobalOnDeath = GameManager.Instance.PlayerDeath;
    StartCoroutine(SpawnRoutine());
  }
}