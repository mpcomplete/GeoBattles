using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState {
  PreGame,
  Loading,
  InGame,
  PostGame,
}

public class GameManager : SingletonBehavior<GameManager> {
  [RuntimeInitializeOnLoadMethod]
  public static void Boot() {
    Time.fixedDeltaTime = 1f/60f;
  }

  public GameState GameState;
  public bool IsGameActive => GameState == GameState.InGame;
  public List<Character> Players;
  public List<Character> Mobs;
  public List<GameObject> VFX;
  public List<Projectile> Projectiles;
  public List<GridForce> GridForces;
  public List<BlackHoleTarget> BlackHoleTargets;

  public UnityAction<Character> PlayerSpawn;
  public UnityAction<Character> PlayerAlive;
  public UnityAction<Character> PlayerDying;
  public UnityAction<Character> PlayerDeath;
  public UnityAction<Character> PlayerDespawn;
  public UnityAction<Character> MobSpawn;
  public UnityAction<Character> MobAlive;
  public UnityAction<Character> MobDying;
  public UnityAction<Character> MobDeath;
  public UnityAction<Character> MobDespawn;
  public UnityAction<Projectile> ProjectileSpawn;
  public UnityAction<Projectile> ProjectileDeath;
  public UnityAction PreGame;
  public UnityAction StartGame;
  public UnityAction PostGame;
  public UnityAction LevelChange;

  protected override void AwakeSingleton() {
    Players = new();
    Mobs = new();
    VFX = new();
    Projectiles = new();
    PlayerSpawn += OnPlayerSpawn;
    PlayerDying += OnPlayerDying;
    MobSpawn += OnMobSpawn;
    MobDespawn += OnMobDespawn;
    MobDeath += OnMobDeath;
    ProjectileSpawn += OnProjectileSpawn;
    ProjectileDeath += OnProjectileDeath;
    PreGame += OnPreGame;
    StartGame += OnLevelStart;
    PostGame += OnLevelEnd;
  }

  void Start() {
    LoadMainMenu();
  }

  public void LoadMainMenu() {
    PreGame.Invoke();
  }

  public void LoadLevel() {
    StartCoroutine(LoadNextLevel());
  }

  IEnumerator LoadNextLevel() {
    var load = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    GameState = GameState.Loading;
    while (!load.isDone) {
      yield return null;
    }
    LevelChange?.Invoke();
    StartGame?.Invoke();
  }

  // Despawning mobs removes them from the list
  public void DespawnMobsSafe(Predicate<Character> predicate) {
    for (var i = Mobs.Count - 1; i >= 0; i--) {
      if (predicate(Mobs[i])) {
        Mobs[i].Despawn();
      }
    }
  }

  void OnPlayerSpawn(Character character) {
    DespawnMobsSafe(c => true);
    Players.Add(character);
  }

  void OnPlayerDying(Character character) {
    DespawnMobsSafe(c => c != character.LastAttacker);
    Players.Remove(character);
  }

  void OnMobSpawn(Character character) {
    Mobs.Add(character);
  }

  void OnMobDespawn(Character character) {
    Mobs.Remove(character);
  }

  void OnMobDeath(Character character) {
    Mobs.Remove(character);
  }

  void OnProjectileSpawn(Projectile p) {
    Projectiles.Add(p);
  }

  void OnProjectileDeath(Projectile p) {
    Projectiles.Remove(p);
  }

  void OnPreGame() {
    DespawnMobsSafe(c => true);
    GameState = GameState.PreGame;
  }

  void OnLevelStart() {
    DespawnMobsSafe(c => true);
    GameState = GameState.InGame;
  }

  void OnLevelEnd() {
    DespawnMobsSafe(c => true);
    GameState = GameState.PostGame;
  }
}