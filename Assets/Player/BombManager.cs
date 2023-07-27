using UnityEngine;
using UnityEngine.Events;

public class BombManager : LevelManager<BombManager> {
  [SerializeField] int BonusBombScoreInterval = 100000;
  [SerializeField] int InitialBombCount = 3;

  int BombCount = 3;

  public UnityAction<int> BombCountChange;

  public bool TryDetonateBomb() {
    if (BombCount > 0) {
      BombCount -= 1;
      BombCountChange?.Invoke(BombCount);
      return true;
    } else {
      return false;
    }
  }

  void AwakeSingleton() {
    GameManager.Instance.StartGame += SetBombCount;
  }

  void Start() {
    ScoreManager.Instance.ScoreChange += TryAwardExtraBomb;
  }

  void OnDestroy() {
    GameManager.Instance.StartGame -= SetBombCount;
    ScoreManager.Instance.ScoreChange -= TryAwardExtraBomb;
  }

  void SetBombCount() {
    BombCount = InitialBombCount;
    BombCountChange?.Invoke(BombCount);
  }

  void TryAwardExtraBomb(ScoreEvent scoreEvent) {
    var previousScore = scoreEvent.ScoreTotal-scoreEvent.ScoreChange;
    var newScore = scoreEvent.ScoreTotal;
    if ((previousScore / BonusBombScoreInterval) < (newScore / BonusBombScoreInterval)) {
      BombCount += 1;
      BombCountChange?.Invoke(BombCount);
    }
  }
}