using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  public float PostDelay = 1f;
  public int ChooseWeight = 10;
  public float TimeFirstAvailable = 0f;
  public float TimeLastAvailable = Mathf.Infinity;

  // TODO: scaling
  float ScaledDelayBetweenMobs => DelayBetweenMobs;
  float ScaledPostDelay => PostDelay;
  int ScaledNumMobs => NumMobs;

  public IEnumerator SpawnSequence() {
    Chosen = null;
    var sequence = Location switch {
      Locations.Random => SpawnRandom(),
      Locations.Surrounding => SpawnSurrounding(),
      Locations.Corners => SpawnCorners(),
      _ => null
    };
    yield return sequence;
    yield return new WaitForSeconds(ScaledPostDelay);
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
    for (int i = 0; i < ScaledNumMobs; i++) {
      SpawnMob(GetPos());
      yield return WaveDelay();
    }
    if (IncludeBlackHole)
      SpawnBlackHole(GetPos());
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
    for (int i = 0; i < ScaledNumMobs; i++) {
      SpawnMob(GetPos());
      yield return WaveDelay();
    }
    if (IncludeBlackHole)
      SpawnBlackHole(GetPos());
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
    for (int i = 0; i < ScaledNumMobs; i++) {
      corners.ForEach(c => SpawnMob(GetPos(c)));
      yield return WaveDelay();
    }
    if (IncludeBlackHole)
      corners.ForEach(c => SpawnBlackHole(GetPos(c)));
  }

  public IEnumerator WaveDelay() {
    if (ScaledDelayBetweenMobs > 0f)
      yield return new WaitForSeconds(ScaledDelayBetweenMobs);
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