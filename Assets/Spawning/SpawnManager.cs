using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
  public static SpawnManager Instance;

  // Every X seconds, incrase a parameter.
  public float SpewRateIncreasePeriod = 60f;
  public float SpawnDelayDecreasePeriod = 60f;
  public float NumMobsIncreasePeriod = 60f;

  SpawnEvent[] SpawnEvents;
  SpawnEvent CurrentSpawnEvent;
  public float DebugStartTime = 0f;

  public float CurrentTime => Time.time + DebugStartTime;
  public float SpewDelayFactor => 1f / (1f + CurrentTime / SpewRateIncreasePeriod);
  public float SpawnDelayFactor => 1f / (1f + CurrentTime / SpawnDelayDecreasePeriod);
  public float NumMobsFactor => 1f + CurrentTime / NumMobsIncreasePeriod;

  void Awake() {
    Instance = this;
    SpawnEvents = GetComponentsInChildren<SpawnEvent>();
    GameManager.Instance.PlayerAlive += OnPlayerAlive;
    GameManager.Instance.PlayerDying += OnPlayerDying;
    StartCoroutine(RunEventLoop());
  }

  void OnDestroy() {
    Instance = null;
    GameManager.Instance.PlayerAlive -= OnPlayerAlive;
    GameManager.Instance.PlayerDying -= OnPlayerDying;
    StopAllCoroutines();
  }

  public bool PlayerAlive = false;
  void OnPlayerAlive(Character c) {
    StartCoroutine(WaitThenEnable(1));
  }
  void OnPlayerDying(Character c) {
    PlayerAlive = false;
  }

  IEnumerator WaitThenEnable(float seconds) {
    yield return new WaitForSeconds(1);
    PlayerAlive = true;
  }

  IEnumerator RunEventLoop() {
    yield return new WaitUntil(() => PlayerAlive);
    CurrentSpawnEvent = ChooseEvent();
    Debug.Log($"Spawn event starting {CurrentTime}: spewrate={SpewDelayFactor}, spawndelay={SpawnDelayFactor}, nummobs={NumMobsFactor}");
    yield return CurrentSpawnEvent.SpawnSequence();
    CurrentSpawnEvent = null;
    StartCoroutine(RunEventLoop());
  }

  SpawnEvent ChooseEvent() {
    var validEvents = SpawnEvents.Where(e => CurrentTime >= e.TimeFirstAvailable && CurrentTime < e.TimeLastAvailable).ToArray();
    var totalScore = validEvents.Sum(e => e.ChooseWeight);
    var choose = UnityEngine.Random.Range(0, totalScore);
    var sum = 0;
    foreach (var e in validEvents) {
      sum += e.ChooseWeight;
      if (choose < sum) return e;
    }
    Debug.Assert(false);
    return null;
  }
}