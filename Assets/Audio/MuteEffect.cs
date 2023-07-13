using System.Collections;
using UnityEngine;

public class MuteEffect : MonoBehaviour {
  [SerializeField] AnimationCurve VolumeCurve;
  [SerializeField] float Duration;

  IEnumerator Start() {
    transform.SetParent(null);
    float t = 0f;
    var oldVolume = AudioListener.volume;
    while (t <= Duration) {
      AudioListener.volume = oldVolume * VolumeCurve.Evaluate(t / Duration);
      t += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    AudioListener.volume = oldVolume;
    Destroy(gameObject);
  }
}