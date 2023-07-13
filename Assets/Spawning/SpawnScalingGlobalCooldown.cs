using UnityEngine;

public class SpawnScalingGlobalCooldown : SpawnIncreaseParam {
  public float DecreaseBy = .002f;

  protected override void SetParam() {
    SpawnEvent.GlobalCooldown -= DecreaseBy;
    //Debug.Log($"Decreasing {SpawnEvent.name} GlobalCooldown to {SpawnEvent.GlobalCooldown}");
  }
}