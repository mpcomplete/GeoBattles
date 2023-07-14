using UnityEngine;
using UnityEngine.Events;

public struct ScoreEvent {
  public Vector3 Position;
  public int ScoreChange;
  public int ScoreTotal;
  public ScoreEvent(Vector3 position, int scoreChange, int scoreTotal) {
    Position = position;
    ScoreChange = scoreChange;
    ScoreTotal = scoreTotal;
  }
}

public struct MultiplierEvent {
  public Vector3 Position;
  public int Multiplier;
  public MultiplierEvent(Vector3 position, int multiplier) {
    Position = position;
    Multiplier = multiplier;
  }
}

public class ScoreManager : LevelManager<ScoreManager> {
  public int HighScore {
    get => PlayerPrefs.GetInt("HighScore", 0);
    set => PlayerPrefs.SetInt("HighScore", value);
  }
  public int Score;
  public int Multiplier;
  public int ThresholdBase = 20;
  public int Kills;
  public int Threshold => ThresholdBase * (int)Mathf.Pow(2, Multiplier-1);

  public UnityAction<int> SetScore;
  public UnityAction<int> SetHighScore;
  public UnityAction<ScoreEvent> ScoreChange;
  public UnityAction<ScoreEvent> HighScoreChange;
  public UnityAction<int> SetMultiplier;
  public UnityAction<MultiplierEvent> MultiplierChange;

  [ContextMenu("Reset High Score")]
  void ResetHighScore() {
    HighScore = 0;
  }

  protected override void Awake() {
    base.Awake();
    GameManager.Instance.MobDying += OnMobDying;
    GameManager.Instance.StartGame += StartGame;
    GameManager.Instance.PlayerSpawn += ResetMultiplier;
  }

  void OnDestroy() {
    GameManager.Instance.MobDying -= OnMobDying;
    GameManager.Instance.StartGame -= StartGame;
    GameManager.Instance.PlayerSpawn -= ResetMultiplier;
  }

  void StartGame() {
    Kills = 0;
    Score = 0;
    Multiplier = 1;
    SetScore?.Invoke(Score);
    SetHighScore?.Invoke(HighScore);
    SetMultiplier?.Invoke(Multiplier);
  }

  void ResetMultiplier(Character c) {
    Kills = 0;
    Multiplier = 1;
    SetMultiplier?.Invoke(Multiplier);
  }

  void OnMobDying(Character character) {
    var mob = character as Mob;
    var scoreChange = mob.Score * Multiplier;
    var position = character.transform.position;
    Score += scoreChange;
    Kills += 1;
    ScoreChange?.Invoke(new(position, scoreChange, Score));
    if (Score > HighScore) {
      HighScore = Score;
      HighScoreChange?.Invoke(new(position, scoreChange, Score));
    }
    if (Kills >= Threshold) {
      Kills = 0;
      Multiplier += 1;
      MultiplierChange?.Invoke(new(position, Multiplier));
    }
  }
}