using UnityEngine;

public static class AudioExtensions {
  public static void Play(this AudioSource source, AudioClip clip) {
    source.Stop();
    source.clip = clip;
    source.Play();
  }
}

public class AudioManager : MonoBehaviour {
  [SerializeField] AudioClip MenuMusic;
  [SerializeField] AudioClip InGameMusic;
  [SerializeField] AudioClip GameOverMusic;
  [SerializeField] AudioSource MusicSource;
  [SerializeField] AudioSource PlayerSource;
  [SerializeField] AudioSource MobSource;

  void Start() {
    GameManager.Instance.LevelStart += LevelStart;
    GameManager.Instance.LevelEnd += LevelEnd;
  }

  void OnDestroy() {
    base.SendMessage("OnDestroy");
    GameManager.Instance.LevelStart -= LevelStart;
    GameManager.Instance.LevelEnd -= LevelEnd;
  }

  void LevelStart() => MusicSource.Play(InGameMusic);

  void LevelEnd() => MusicSource.Play(GameOverMusic);
}