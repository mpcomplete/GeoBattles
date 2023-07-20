using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShipManager : LevelManager<ShipManager> {
  [SerializeField] Player PlayerPrefab;
  [SerializeField] int BonusShipScoreInterval = 75000;
  [SerializeField] int InitialShipCount = 3;

  int ShipCount;
  Vector3 RespawnPosition;

  public UnityAction<int> ShipCountChange;

  protected override void Awake() {
    base.Awake();
    GameManager.Instance.PlayerDying += ShipDying;
    GameManager.Instance.PlayerDeath += ShipDeath;
    GameManager.Instance.StartGame += StartGame;
  }

  void Start() {
    ScoreManager.Instance.ScoreChange += TryAwardExtraShip;
  }

  void OnDestroy() {
    GameManager.Instance.PlayerDying -= ShipDying;
    GameManager.Instance.PlayerDeath -= ShipDeath;
    GameManager.Instance.StartGame -= StartGame;
    ScoreManager.Instance.ScoreChange -= TryAwardExtraShip;
  }

  void StartGame() {
    var player = Instantiate(PlayerPrefab, transform);
    player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    ShipCount = InitialShipCount;
    ShipCountChange?.Invoke(ShipCount);
  }

  void ShipDying(Character c) {
    RespawnPosition = c.transform.position;
  }

  void ShipDeath(Character c) {
    if (ShipCount <= 0) {
      GameManager.Instance.PostGame?.Invoke();
    } else {
      ShipCount -= 1;
      ShipCountChange?.Invoke(ShipCount);
      StartCoroutine(Respawn(RespawnPosition, Quaternion.identity));
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

  IEnumerator Respawn(Vector3 position, Quaternion rotation) {
    yield return new WaitForSeconds(1f);
    GameManager.Instance.DespawnMobsSafe(c => true) ;
    yield return new WaitForSeconds(1f);
    var player = Instantiate(PlayerPrefab, transform);
    player.transform.SetPositionAndRotation(position, rotation);
  }
}