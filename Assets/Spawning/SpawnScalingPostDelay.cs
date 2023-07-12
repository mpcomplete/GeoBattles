using UnityEngine;

public class SpawnScalingPostDelay : SpawnIncreaseParam {
  public float DecreaseBy = .002f;

  protected override void SetParam() {
    SpawnEvent.PostDelay += DecreaseBy;
    Debug.Log($"Decreasing {SpawnEvent.name} PostDelay to {SpawnEvent.PostDelay}");
  }
}