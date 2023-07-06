using UnityEngine;
using TMPro;

public class UI : MonoBehaviour {
  [SerializeField] RectTransform ShipContainer;
  [SerializeField] RectTransform BombContainer;
  [SerializeField] TextMeshProUGUI Score;
  [SerializeField] TextMeshProUGUI HighScore;

  void Awake() {
    ScoreManager.Instance.SetScore += SetScore;
    ScoreManager.Instance.SetHighScore += SetHighScore;
    ScoreManager.Instance.ScoreChange += ScoreChange;
    ScoreManager.Instance.HighScoreChange += HighScoreChange;
  }

  void OnDestroy() {
    ScoreManager.Instance.SetScore -= SetScore;
    ScoreManager.Instance.SetHighScore -= SetHighScore;
    ScoreManager.Instance.ScoreChange -= ScoreChange;
    ScoreManager.Instance.HighScoreChange -= HighScoreChange;
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
}
