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

  [Header("Testing/Encounter")]
  public Timeval SpawnPeriod = Timeval.FromSeconds(1);
  public int MobsSpawnedPerWave = 1;
  int SpawnTicksRemaining;
  bool IsEncounterActive = false;

  public List<Character> Players;
  public List<Character> Mobs;
  public List<GameObject> VFX;
  public List<Projectile> Projectiles;
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
    DespawnMobsSafe(c => true);
    IsEncounterActive = true;
  }

  void OnPlayerDying(Character character) {
    // Steve: This is pretty hacky way to avoid collision between new ship and last attackers hitbox
    // This could be a whole "state" of a mob called Suspended.
    // In GW, these mobs have no motion and they flash for awhile before being removed (not even despawned)
    if (character.LastAttacker)
      character.LastAttacker.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false);
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

  void FixedUpdate() {
    if (--SpawnTicksRemaining <= 0 && IsEncounterActive) {
      for (var i = 0; i < MobsSpawnedPerWave; i++) {
        var prefab = MobPrefabs.Random();
        var position = 10 * UnityEngine.Random.onUnitSphere.XZ();
        Instantiate(prefab, position, Quaternion.identity);
        SpawnTicksRemaining = SpawnPeriod.Ticks;
      }
    }
  }
}