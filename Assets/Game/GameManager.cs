using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Score {
  public float Multiplier;
  public float CurrentScore;
}

public enum GameEvent {
  PlayerSpawn,
  PlayerAlive,
  PlayerDying,
  PlayerDeath,
  MobSpawn,
  MobAlive,
  MobDying,
  MobDeath,
  LevelStart,
  GameOver,
  HighScoreChange,
}

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  [RuntimeInitializeOnLoadMethod]
  public static void Boot() {
    Debug.LogWarning("GameManager.Boot sets fixed frame rate.");
    Time.fixedDeltaTime = 1f/60f;
  }

  [SerializeField] Character PlayerPrefab;
  [SerializeField] Character[] MobPrefabs;

  [Header("Testing/Encounter")]
  public Timeval SpawnPeriod = Timeval.FromSeconds(1);
  public int MobsSpawnedPerWave = 1;
  int SpawnTicksRemaining;

  public float HighScore;
  public Score Score;
  public List<Character> Players;
  public List<Character> Mobs;
  public List<GameObject> VFX;
  public List<Projectile> Projectiles;

  public UnityAction<Character> PlayerSpawn;
  public UnityAction<Character> PlayerAlive;
  public UnityAction<Character> PlayerDying;
  public UnityAction<Character> PlayerDeath;
  public UnityAction<Character> MobSpawn;
  public UnityAction<Character> MobAlive;
  public UnityAction<Character> MobDying;
  public UnityAction<Character> MobDeath;
  public UnityAction LevelStart;
  public UnityAction LevelEnd;
  public UnityAction<float> HighScoreChange;

  void OnPlayerSpawn(Character character) {
    Players.Add(character);
  }

  void OnPlayerDeath(Character character) {
    Players.Remove(character);
  }

  void OnMobSpawn(Character character) {
    Mobs.Add(character);
  }

  void OnMobDeath(Character character) {
    Mobs.Remove(character);
  }

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Players = new();
      Mobs = new();
      VFX = new();
      Projectiles = new();
      Instance = this;
      PlayerSpawn += OnPlayerSpawn;
      PlayerDeath += OnPlayerDeath;
      MobSpawn += OnMobSpawn;
      MobDeath += OnMobDeath;
      DontDestroyOnLoad(gameObject);
    }
  }

  void FixedUpdate() {
    if (SpawnTicksRemaining > 0) {
      SpawnTicksRemaining--;
    } else {
      for (var i = 0; i < MobsSpawnedPerWave; i++) {
        var prefab = MobPrefabs.Random();
        var position = 10 * UnityEngine.Random.onUnitSphere.XZ();
        Instantiate(prefab, position, Quaternion.identity);
        SpawnTicksRemaining = SpawnPeriod.Ticks;
      }
    }
  }
}