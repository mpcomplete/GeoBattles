using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UI : SingletonBehavior<UI> {
  [Header("HUD")]
  [SerializeField] RectTransform HUD;
  [SerializeField] RectTransform ShipContainer;
  [SerializeField] RectTransform BombContainer;
  [SerializeField] TextMeshProUGUI Score;
  [SerializeField] TextMeshProUGUI HighScore;
  [SerializeField] RectTransform BombIconPrefab;
  [SerializeField] RectTransform ShipIconPrefab;
  [SerializeField] TextMeshProUGUI CountPrefab;

  [Header("GameOver")]
  [SerializeField] RectTransform GameOverMenu;
  [SerializeField] TextMeshProUGUI FinalScore;

  [Header("Pause")]
  [SerializeField] RectTransform PauseMenu;

  [Header("Main Menu")]
  [SerializeField] RectTransform MainMenu;
  [SerializeField] Button PlayButton;

  protected override void AwakeSingleton() {
    GameManager.Instance.PreGame += PreGame;
    GameManager.Instance.StartGame += StartGame;
    GameManager.Instance.PostGame += PostGame;
    GameManager.Instance.LevelChange += LevelChange;
    PauseManager.Instance.OnPause += Pause;
    PauseManager.Instance.OnUnpause += UnPause;
  }

  void LevelChange() {
    ScoreManager.Instance.SetScore += SetScore;
    ScoreManager.Instance.SetHighScore += SetHighScore;
    ScoreManager.Instance.ScoreChange += ScoreChange;
    ScoreManager.Instance.HighScoreChange += HighScoreChange;
    ShipManager.Instance.ShipCountChange += ShipCountChange;
    BombManager.Instance.BombCountChange += BombCountChange;
  }

  void PreGame() {
    HUD.gameObject.SetActive(false);
    MainMenu.gameObject.SetActive(true);
    GameOverMenu.gameObject.SetActive(false);
    PauseMenu.gameObject.SetActive(false);
    PlayButton.onClick.AddListener(OnPlay);
    EventSystem.current.SetSelectedGameObject(PlayButton.gameObject);
  }

  void OnPlay() {
    GameManager.Instance.LoadLevel();
  }

  void PostGame() {
    HUD.gameObject.SetActive(true);
    MainMenu.gameObject.SetActive(false);
    GameOverMenu.gameObject.SetActive(true);
    PauseMenu.gameObject.SetActive(false);
    FinalScore.text = $"Final Score: {ScoreManager.Instance.Score.ToString("N0")}";
  }

  void StartGame() {
    HUD.gameObject.SetActive(true);
    MainMenu.gameObject.SetActive(false);
    GameOverMenu.gameObject.SetActive(false);
    PauseMenu.gameObject.SetActive(false);
  }

  void Pause() {
    PauseMenu.gameObject.SetActive(true);
  }

  void UnPause() {
    PauseMenu.gameObject.SetActive(false);
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