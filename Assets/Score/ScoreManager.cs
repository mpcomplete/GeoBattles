using UnityEngine;
using UnityEngine.Events;

public struct ScoreEvent {
  public Character Character;
  public int Multiplier;
  public ScoreEvent(Character character, int multiplier) {
    Character = character;
    Multiplier = multiplier;
  }
}

public class ScoreManager : MonoBehaviour {
  public static ScoreManager Instance;

  public int Score;
  public int Multiplier;
  public int ThresholdBase = 20;
  public int Kills;
  public int Threshold => ThresholdBase * (int)Mathf.Pow(2, Multiplier-1);

  public UnityAction<ScoreEvent> MultiplierChange;
  public UnityAction<ScoreEvent> ScoreChange;

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    GameManager.Instance.MobDying += OnMobDying;
  }

  void OnMobDying(Character character) {
    var mob = character as Mob;
    Score += mob.Score * Multiplier;
    Kills += 1;
    ScoreChange?.Invoke(new(character, Multiplier));
    if (Kills >= Threshold) {
      Multiplier += 1;
      Kills = 0;
      MultiplierChange?.Invoke(new(character, Multiplier));
    }
  }
}