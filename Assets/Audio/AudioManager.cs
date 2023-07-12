using UnityEngine;

public static class AudioExtensions {
  public static void Play(this AudioSource source, AudioClip clip) {
    source.Stop();
    source.clip = clip;
    source.Play();
  }
}

public class AudioManager : SingletonBehavior<AudioManager> {
  [SerializeField] AudioClip MenuMusic;
  [SerializeField] AudioClip InGameMusic;
  [SerializeField] AudioClip GameOverMusic;
  [SerializeField] AudioSource MusicSource;
  [SerializeField] AudioSource PlayerSource;
  [SerializeField] AudioSource MobSource;
  [SerializeField] AudioSource ProjectileSource;

  protected override void AwakeSingleton() {
    GameManager.Instance.LevelStart += LevelStart;
    GameManager.Instance.LevelEnd += LevelEnd;
  }

  void LevelStart() => MusicSource.Play(InGameMusic);

  void LevelEnd() => MusicSource.Play(GameOverMusic);
}