using UnityEngine;
using UnityEngine.Events;

public class ShipManager : MonoBehaviour {
  public static ShipManager Instance;

  public int BonusShipScoreInterval = 75000;
  public int ShipCount = 3;
  public UnityAction<int> ShipCountChange;

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
      GameManager.Instance.PlayerDying += ShipDeath;
      GameManager.Instance.LevelStart += SetShipCount;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    ScoreManager.Instance.ScoreChange += TryAwardExtraShip;
  }

  void OnDestroy() {
    GameManager.Instance.PlayerDying -= ShipDeath;
    GameManager.Instance.LevelStart -= SetShipCount;
    ScoreManager.Instance.ScoreChange -= TryAwardExtraShip;
  }

  void SetShipCount() {
    ShipCountChange?.Invoke(ShipCount);
  }

  void ShipDeath(Character c) {
    ShipCount = Mathf.Max(0, ShipCount-1);
    if (ShipCount <= 0) {
      GameManager.Instance.LevelEnd?.Invoke();
    }
  }

  void TryAwardExtraShip(ScoreEvent scoreEvent) {
    var previousScore = scoreEvent.ScoreTotal-scoreEvent.ScoreChange;
    var newScore = scoreEvent.ScoreTotal;
    if ((previousScore / BonusShipScoreInterval) < (newScore / BonusShipScoreInterval)) {
      ShipCount += 1;
      ShipCountChange?.Invoke(ShipCount);
    }
  }
}