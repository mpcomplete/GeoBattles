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
    for (int i = 0; i < ScaledNumMobs; i++) {
      var player = GameManager.Instance.Players[0];
      var b = Bounds.Instance;
      Vector3 randomPos;
      do {
        randomPos = new Vector3(UnityEngine.Random.Range(b.XMin + Radius, b.XMax - Radius), 0f, UnityEngine.Random.Range(b.ZMin + Radius, b.ZMax - Radius));
      } while ((randomPos - player.transform.position).sqrMagnitude < MinPlayerDistance);
      SpawnMob(randomPos);
      yield return new WaitForSeconds(ScaledDelayBetweenMobs);
    }
  }

  private IEnumerator SpawnSurrounding() {
    const float MinDistance = 5f;
    const float MaxDistance = 7f;
    for (int i = 0; i < ScaledNumMobs; i++) {
      var player = GameManager.Instance.Players[0];
      var b = Bounds.Instance;
      var randomPos = player.transform.position + UnityEngine.Random.onUnitSphere.XZ().normalized * UnityEngine.Random.Range(MinDistance, MaxDistance);
      randomPos = Bounds.Instance.Bound(randomPos, 1f);
      SpawnMob(randomPos);
      yield return new WaitForSeconds(ScaledDelayBetweenMobs);
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
    for (int i = 0; i < ScaledNumMobs; i++) {
      foreach (var corner in corners) {
        var pos = corner + UnityEngine.Random.insideUnitCircle.XZ();
        SpawnMob(pos);
      }
      yield return new WaitForSeconds(ScaledDelayBetweenMobs);
    }
  }

  void SpawnMob(Vector3 randomPos) {
    var mob = ChooseMob();
    Instantiate(mob, randomPos, Quaternion.identity);
  }

  GameObject Chosen = null;
  GameObject ChooseMob() {
    if (Chosen == null || DifferentMob) Chosen = Spawn.Choose();
    return Chosen;
  }
}