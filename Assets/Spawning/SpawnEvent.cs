using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SpawnList {
  public List<GameObject> Possible;
  public GameObject Choose() {
    return Possible.Random();
  }
}

public class SpawnEvent : MonoBehaviour {
  public enum Locations { Random, Surrounding, Corners };
  public Locations Location;
  public SpawnList Spawn;
  public GameObject IncludeBlackHole;
  public int NumMobs = 1;
  public float DelayBetweenMobs = .1f;
  public bool DifferentMob = false;
  [Tooltip("Time before any event can run after this, starting from event start")] public float GlobalCooldown = 1f;
  [Tooltip("Time before this event can be chosen again, starting from event start")] public float SelfCooldown = 0f;
  public int ChooseWeight = 10;
  public float TimeFirstAvailable = 0f;
  public float TimeLastAvailable = Mathf.Infinity;

  public IEnumerator SpawnSequence() {
    Chosen = null;
    var sequence = Location switch {
      Locations.Random => SpawnRandom(),
      Locations.Surrounding => SpawnSurrounding(),
      Locations.Corners => SpawnCorners(),
      _ => null
    };
    StartCoroutine(sequence);
    Debug.Log($"At t={SpawnManager.Instance.CurrentTime}, event {name} running (mobs={NumMobs}, between={DelayBetweenMobs}). Waiting={GlobalCooldown}");
    if (SelfCooldown > 0)
      StartCoroutine(DoEventCooldown());
    yield return new WaitForSeconds(GlobalCooldown);
  }

  // Note: This works because disabling a MonoBehaviour does not stop coroutines. Disabling the gameObject does, though.
  IEnumerator DoEventCooldown() {
    enabled = false;
    yield return new WaitForSeconds(SelfCooldown);
    enabled = true;
  }

  const float Radius = 1f;
  private IEnumerator SpawnRandom() {
    const float MinPlayerDistance = 3f;
    Vector3 GetPos() {
      var player = GameManager.Instance.Players[0];
      var b = Bounds.Instance;
      Vector3 pos;
      do {
        pos = new Vector3(UnityEngine.Random.Range(b.XMin + Radius, b.XMax - Radius), 0f, UnityEngine.Random.Range(b.ZMin + Radius, b.ZMax - Radius));
      } while ((pos - player.transform.position).sqrMagnitude < MinPlayerDistance.Sqr());
      return pos;
    }
    if (IncludeBlackHole)
      SpawnBlackHole(GetPos());
    for (int i = 0; i < NumMobs; i++) {
      SpawnMob(GetPos());
      yield return WaveDelay();
    }
  }

  private IEnumerator SpawnSurrounding() {
    const float MinDistance = 8f;
    const float MaxDistance = 10f;
    Vector3 GetPos() {
      var player = GameManager.Instance.Players[0];
      var b = Bounds.Instance;
      var randomPos = player.transform.position + UnityEngine.Random.onUnitSphere.XZ().normalized * UnityEngine.Random.Range(MinDistance, MaxDistance);
      return Bounds.Instance.Bound(randomPos, 1f);
    }
    if (IncludeBlackHole)
      SpawnBlackHole(GetPos());
    for (int i = 0; i < NumMobs; i++) {
      SpawnMob(GetPos());
      yield return WaveDelay();
    }
  }

  private IEnumerator SpawnCorners() {
    var b = Bounds.Instance;
    var corners = new[] {
      new Vector3(b.XMin + Radius, 0, b.ZMin + Radius),
      new Vector3(b.XMin + Radius, 0, b.ZMax - Radius),
      new Vector3(b.XMax - Radius, 0, b.ZMin + Radius),
      new Vector3(b.XMax - Radius, 0, b.ZMax - Radius),
    };
    Vector3 GetPos(Vector3 corner) => corner + UnityEngine.Random.insideUnitCircle.XZ();
    if (IncludeBlackHole)
      corners.ForEach(c => SpawnBlackHole(GetPos(c)));
    for (int i = 0; i < NumMobs; i++) {
      corners.ForEach(c => SpawnMob(GetPos(c)));
      yield return WaveDelay();
    }
  }

  public IEnumerator WaveDelay() {
    if (DelayBetweenMobs > 0f)
      yield return new WaitForSeconds(DelayBetweenMobs);
    // Lame. No way to pause a coroutine so we just have to poll.
    if (!SpawnManager.Instance.PlayerAlive)
      yield return new WaitUntil(() => SpawnManager.Instance.PlayerAlive);
  }

  void SpawnMob(Vector3 randomPos) {
    var mob = ChooseMob();
    Instantiate(mob, randomPos, Quaternion.identity);
  }

  void SpawnBlackHole(Vector3 randomPos) {
    Instantiate(IncludeBlackHole, randomPos, Quaternion.identity);
  }

  GameObject Chosen = null;
  GameObject ChooseMob() {
    if (Chosen == null || DifferentMob) Chosen = Spawn.Choose();
    return Chosen;
  }
}