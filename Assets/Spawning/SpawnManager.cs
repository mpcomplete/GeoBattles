using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
  public SpawnManager Instance;
  SpawnEvent[] SpawnEvents;
  SpawnEvent CurrentSpawnEvent;

  void Awake() {
    Instance = this;
    SpawnEvents = GetComponentsInChildren<SpawnEvent>();
    GameManager.Instance.PlayerAlive += OnPlayerAlive;
  }

  void OnPlayerAlive(Character c) {
    StartCoroutine(RunEventAfterDelay(5));
  }

  IEnumerator RunEventAfterDelay(float seconds) {
    yield return new WaitForSeconds(seconds);
    CurrentSpawnEvent = ChooseEvent();
    yield return CurrentSpawnEvent.SpawnSequence();
    CurrentSpawnEvent = null;
    StartCoroutine(RunEventAfterDelay(0));
  }

  SpawnEvent ChooseEvent() {
    var validEvents = SpawnEvents.Where(e => Time.time >= e.TimeFirstAvailable && Time.time < e.TimeLastAvailable).ToArray();
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