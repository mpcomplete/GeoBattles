using UnityEngine;
using UnityEngine.Events;

public class BombManager : MonoBehaviour {
  public static BombManager Instance;

  [SerializeField] int BonusBombScoreInterval = 100000;
  [SerializeField] int BombCount = 3;

  public UnityAction<int> BombCountChange;

  public bool TryDeonateBomb() {
    if (BombCount > 0) {
      BombCount -= 1;
      BombCountChange?.Invoke(BombCount);
      return true;
    } else {
      return false;
    }
  }

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
      GameManager.Instance.LevelStart += SetBombCount;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    ScoreManager.Instance.ScoreChange += TryAwardExtraBomb;
  }

  void OnDestroy() {
    GameManager.Instance.LevelStart -= SetBombCount;
    ScoreManager.Instance.ScoreChange -= TryAwardExtraBomb;
  }

  void SetBombCount() {
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