using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MuteEffect : MonoBehaviour {
  [SerializeField] AudioMixer AudioMixer;
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
    AudioMixer.GetFloat("MusicVolume", out var oldMusic);
    AudioMixer.GetFloat("SoundVolume", out var oldSound);
    while (T < endTime) {
      var factor = (VolumeCurve.Evaluate(T)-1)*80f;  // map [0,1] to [-80db, 0db]
      AudioMixer.SetFloat("MusicVolume", oldMusic + factor);
      AudioMixer.SetFloat("SoundVolume", oldSound + factor);
      AudioMixer.GetFloat("MusicVolume", out var tmp);
      T += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    AudioMixer.SetFloat("MusicVolume", oldMusic);
    AudioMixer.SetFloat("SoundVolume", oldSound);
    RunningInstance = null;
    Destroy(gameObject);
  }
}