using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
  public static SpawnManager Instance;

  public float DebugStartTime = 0f;
  public float CurrentTime => Time.time + DebugStartTime;

  SpawnEvent[] SpawnEvents;
  SpawnEvent CurrentSpawnEvent;

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
    if (CurrentSpawnEvent == null) {  // Nothing available atm, wait and try again.
      yield return new WaitForSeconds(1);
      StartCoroutine(RunEventLoop());
      yield break;
    }
    yield return CurrentSpawnEvent.SpawnSequence();
    CurrentSpawnEvent = null;
    StartCoroutine(RunEventLoop());
  }

  SpawnEvent ChooseEvent() {
    var validEvents = SpawnEvents.Where(e => e.isActiveAndEnabled && CurrentTime >= e.TimeFirstAvailable && CurrentTime < e.TimeLastAvailable).ToArray();
    var totalScore = validEvents.Sum(e => e.ChooseWeight);
    var choose = UnityEngine.Random.Range(0, totalScore);
    var sum = 0;
    foreach (var e in validEvents) {
      sum += e.ChooseWeight;
      if (choose < sum) return e;
    }
    return null;
  }
}