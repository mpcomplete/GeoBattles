using UnityEngine;

public class SpawnIncreaseParam : MonoBehaviour {
  public float Period = 60f;
  public SpawnEvent SpawnEvent => GetComponent<SpawnEvent>();

  private void Start() {
    // Handle starting at time t>0.
    var t = 0f;
    for (t = SpawnManager.Instance.CurrentTime - SpawnEvent.TimeFirstAvailable; t > Period; t -= Period) {
      SetParam();
    }
    Invoke("SetParamRepeat", Mathf.Max(0f, t));
  }

  void SetParamRepeat() {
    SetParam();
    Invoke("SetParamRepeat", Period);
  }

  protected virtual void SetParam() { }
}

public class SpawnScalingNumMobs : SpawnIncreaseParam {
  public int AddMobs;

  protected override void SetParam() {
    SpawnEvent.NumMobs += AddMobs;
    //Debug.Log($"Increasing {SpawnEvent.name} numMobs to {SpawnEvent.NumMobs}");
  }
}