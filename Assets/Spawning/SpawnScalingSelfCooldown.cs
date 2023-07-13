using UnityEngine;

public class SpawnScalingSelfCooldown : SpawnIncreaseParam {
  public float DecreaseBy = .002f;

  protected override void SetParam() {
    SpawnEvent.SelfCooldown -= DecreaseBy;
    SpawnEvent.SelfCooldown = Mathf.Max(SpawnEvent.SelfCooldown, MinValue);
    //Debug.Log($"Decreasing {SpawnEvent.name} SelfCooldown to {SpawnEvent.SelfCooldown}");
  }
}