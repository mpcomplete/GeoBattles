using UnityEngine;
using TMPro;

public class UI : MonoBehaviour {
  [Header("GameOver")]
  [SerializeField] RectTransform GameOver;
  [SerializeField] TextMeshProUGUI FinalScore;

  [Header("HUD")]
  [SerializeField] RectTransform HUD;
  [SerializeField] RectTransform ShipContainer;
  [SerializeField] RectTransform BombContainer;
  [SerializeField] TextMeshProUGUI Score;
  [SerializeField] TextMeshProUGUI HighScore;
  [SerializeField] RectTransform BombIconPrefab;
  [SerializeField] RectTransform ShipIconPrefab;
  [SerializeField] TextMeshProUGUI CountPrefab;

  void Awake() {
    GameManager.Instance.LevelStart += LevelStart;
    GameManager.Instance.LevelEnd += LevelEnd;
    ScoreManager.Instance.SetScore += SetScore;
    ScoreManager.Instance.SetHighScore += SetHighScore;
    ScoreManager.Instance.ScoreChange += ScoreChange;
    ScoreManager.Instance.HighScoreChange += HighScoreChange;
    ShipManager.Instance.ShipCountChange += ShipCountChange;
    BombManager.Instance.BombCountChange += BombCountChange;
  }

  void OnDestroy() {
    GameManager.Instance.LevelStart += LevelStart;
    GameManager.Instance.LevelEnd -= LevelEnd;
    ScoreManager.Instance.SetScore -= SetScore;
    ScoreManager.Instance.SetHighScore -= SetHighScore;
    ScoreManager.Instance.ScoreChange -= ScoreChange;
    ScoreManager.Instance.HighScoreChange -= HighScoreChange;
    ShipManager.Instance.ShipCountChange -= ShipCountChange;
    BombManager.Instance.BombCountChange -= BombCountChange;
  }

  void LevelEnd() {
    GameOver.gameObject.SetActive(true);
    FinalScore.text = $"Final Score: {ScoreManager.Instance.Score}";
  }

  void LevelStart() {
    HUD.gameObject.SetActive(true);
    GameOver.gameObject.SetActive(false);
  }

  void SetScore(int score) {
    Score.text = score.ToString("N0");
  }

  void SetHighScore(int highScore) {
    HighScore.text = highScore.ToString("N0");
  }

  void ScoreChange(ScoreEvent scoreEvent) {
    Score.text = scoreEvent.ScoreTotal.ToString("N0");
  }

  void HighScoreChange(ScoreEvent scoreEvent) {
    HighScore.text = scoreEvent.ScoreTotal.ToString("N0");
  }

  void ShipCountChange(int count) {
    for (var i = 0; i < ShipContainer.childCount; i++) {
      Destroy(ShipContainer.GetChild(i).gameObject);
    }
    if (count <= 3) {
      for (var i = 0; i < count; i++) {
        Instantiate(ShipIconPrefab, ShipContainer);
      }
    } else {
      var countText = Instantiate(CountPrefab, ShipContainer);
      countText.text = count.ToString();
      Instantiate(ShipIconPrefab, ShipContainer);
    }
  }

  void BombCountChange(int count) {
    for (var i = 0; i < BombContainer.childCount; i++) {
      Destroy(BombContainer.GetChild(i).gameObject);
    }
    if (count <= 3) {
      for (var i = 0; i < count; i++) {
        Instantiate(BombIconPrefab, BombContainer);
      }
    } else {
      var countText = Instantiate(CountPrefab, BombContainer);
      countText.text = count.ToString();
      Instantiate(BombIconPrefab, BombContainer);
    }
  }
}