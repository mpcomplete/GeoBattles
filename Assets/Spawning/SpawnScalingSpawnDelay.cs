using UnityEngine;

public class SpawnScalingSpawnDelay : SpawnIncreaseParam {
  public float DecreaseBy = .002f;

  protected override void SetParam() {
    SpawnEvent.DelayBetweenMobs -= DecreaseBy;
    SpawnEvent.DelayBetweenMobs = Mathf.Max(SpawnEvent.DelayBetweenMobs, MinValue);
    //Debug.Log($"Decreasing {SpawnEvent.name} DelayBetweenMobs to {SpawnEvent.DelayBetweenMobs}");
  }
}