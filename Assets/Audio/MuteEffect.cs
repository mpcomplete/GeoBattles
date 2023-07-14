using System.Collections;
using UnityEngine;

public class MuteEffect : MonoBehaviour {
  [SerializeField] AnimationCurve VolumeCurve;
  static MuteEffect RunningInstance;

  float T = 0f;
  IEnumerator Start() {
    float endTime = VolumeCurve.keys[VolumeCurve.length-1].time;
    if (RunningInstance) {
      RunningInstance.T = 0f;
      Destroy(gameObject);
      yield break;
    }
    RunningInstance = this;
    transform.SetParent(null);
    var oldVolume = AudioListener.volume;
    while (T < endTime) {
      AudioListener.volume = oldVolume * VolumeCurve.Evaluate(T);
      T += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    AudioListener.volume = oldVolume;
    RunningInstance = null;
    Destroy(gameObject);
  }
}