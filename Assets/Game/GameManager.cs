using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  [RuntimeInitializeOnLoadMethod]
  public static void Boot() {
    Time.fixedDeltaTime = 1f/60f;
  }

  [SerializeField] Character PlayerPrefab;
  [SerializeField] Character[] MobPrefabs;

  [Header("Testing/Encounter")]
  public Timeval SpawnPeriod = Timeval.FromSeconds(1);
  public int MobsSpawnedPerWave = 1;
  int SpawnTicksRemaining;

  public List<Character> Players;
  public List<Character> Mobs;
  public List<GameObject> VFX;
  public List<Projectile> Projectiles;
  public List<BlackHoleTarget> BlackHoleTargets;

  public UnityAction<Character> PlayerSpawn;
  public UnityAction<Character> PlayerAlive;
  public UnityAction<Character> PlayerDying;
  public UnityAction<Character> PlayerDeath;
  public UnityAction<Character> MobSpawn;
  public UnityAction<Character> MobAlive;
  public UnityAction<Character> MobDying;
  public UnityAction<Character> MobDeath;
  public UnityAction<Character> MobBombed;
  public UnityAction<Projectile> ProjectileSpawn;
  public UnityAction<Projectile> ProjectileDeath;
  public UnityAction LevelStart;
  public UnityAction LevelEnd;

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
      PlayerDeath += OnPlayerDeath;
      MobSpawn += OnMobSpawn;
      MobDeath += OnMobDeath;
      ProjectileSpawn += OnProjectileSpawn;
      ProjectileDeath += OnProjectileDeath;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    LevelStart?.Invoke();
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