using System.Collections.Generic;
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

  public AudioSource MusicSource;
  public AudioSource SoundSource;
  public AudioSource ProjectileSource;
  public AudioSource BombSource;
  public float SoundCooldown = .05f;

  protected override void AwakeSingleton() {
    GameManager.Instance.PreGame += PreGame;
    GameManager.Instance.StartGame += StartGame;
    GameManager.Instance.PostGame += PostGame;
  }

  void PreGame() => MusicSource.Play(MenuMusic);
  void StartGame() => MusicSource.Play(InGameMusic);
  void PostGame() => MusicSource.Play(GameOverMusic);

  Dictionary<AudioClip, float> SoundLastPlayed = new();
  public void PlaySoundWithCooldown(AudioClip clip) {
    if (!clip) return;
    var lastPlayed = SoundLastPlayed.GetValueOrDefault(clip);
    if (Time.time < lastPlayed + SoundCooldown)
      return;
    SoundLastPlayed[clip] = Time.time;
    SoundSource.PlayOneShot(clip);
  }
}