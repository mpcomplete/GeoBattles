using System.Collections;
using UnityEngine;

public class MuteEffect : MonoBehaviour {
  [SerializeField] AnimationCurve VolumeCurve;

  IEnumerator Start() {
    transform.SetParent(null);
    float t = 0f;
    float endTime = VolumeCurve.keys[VolumeCurve.length-1].time;
    var oldVolume = AudioListener.volume;
    while (t < endTime) {
      AudioListener.volume = oldVolume * VolumeCurve.Evaluate(t);
      t += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    AudioListener.volume = oldVolume;
    Destroy(gameObject);
  }
}