using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  [RuntimeInitializeOnLoadMethod]
  public static void Boot() {
    Time.fixedDeltaTime = 1f/60f;
  }

  [SerializeField] Character[] MobPrefabs;

  public SpringGrid SpringGrid;

  [Header("Testing/Encounter")]
  public Timeval SpawnPeriod = Timeval.FromSeconds(1);
  public int MobsSpawnedPerWave = 1;
  public int SafeSpotSearchAttempts = 100;
  public float MinimumSafeSpawnDistance = 2f;
  int SpawnTicksRemaining;
  bool IsEncounterActive = false;

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
  public UnityAction LevelStart;
  public UnityAction LevelEnd;

  // Despawning mobs removes them from the list
  public void DespawnMobsSafe(Predicate<Character> predicate) {
    for (var i = Mobs.Count - 1; i >= 0; i--) {
      if (predicate(Mobs[i])) {
        Mobs[i].Despawn();
      }
    }
  }

  void OnPlayerSpawn(Character character) {
    SpawnTicksRemaining = SpawnPeriod.Ticks;
    Players.Add(character);
    IsEncounterActive = true;
  }

  void OnPlayerDying(Character character) {
    DespawnMobsSafe(c => c != character.LastAttacker);
    Players.Remove(character);
    IsEncounterActive = false;
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

  void OnLevelEnd() {
    Debug.Log("Game over");
  }

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
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
      LevelEnd += OnLevelEnd;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    LevelStart?.Invoke();
  }

  Vector3 FindRandomPositionAwayFromPlayer() {
    var position = Vector3.zero;
    for (var i = 0; i < 100; i++) {
      position = 10 * UnityEngine.Random.onUnitSphere.XZ();
      if (Players.Count <= 0 || Vector3.Distance(Players[0].transform.position, position) > MinimumSafeSpawnDistance) {
        return position;
      }
    }
    return position;
  }

  void FixedUpdate() {
    ++Timeval.TickCount;
    if (--SpawnTicksRemaining <= 0 && IsEncounterActive) {
      for (var i = 0; i < MobsSpawnedPerWave; i++) {
        var prefab = MobPrefabs.Random();
        var position = FindRandomPositionAwayFromPlayer();
        Instantiate(prefab, position, Quaternion.identity);
        SpawnTicksRemaining = SpawnPeriod.Ticks;
      }
    }
  }
}