using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShipManager : MonoBehaviour {
  public static ShipManager Instance;

  [SerializeField] Player PlayerPrefab;
  [SerializeField] int BonusShipScoreInterval = 75000;
  [SerializeField] int InitialShipCount = 3;

  int ShipCount;

  public UnityAction<int> ShipCountChange;

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
      GameManager.Instance.PlayerDeath += ShipDeath;
      GameManager.Instance.StartGame += StartGame;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    ScoreManager.Instance.ScoreChange += TryAwardExtraShip;
  }

  void OnDestroy() {
    GameManager.Instance.PlayerDeath -= ShipDeath;
    GameManager.Instance.StartGame -= StartGame;
    ScoreManager.Instance.ScoreChange -= TryAwardExtraShip;
  }

  void StartGame() {
    Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
    ShipCount = InitialShipCount;
    ShipCountChange?.Invoke(ShipCount);
  }

  void ShipDeath(Character c) {
    if (ShipCount <= 0) {
      GameManager.Instance.PostGame?.Invoke();
    } else {
      ShipCount -= 1;
      ShipCountChange?.Invoke(ShipCount);
      StartCoroutine(Respawn(c.transform.position, c.transform.rotation));
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
    // yield return new WaitForFixedUpdate();
    yield return new WaitForSeconds(1f);
    Instantiate(PlayerPrefab, position, rotation);
  }
}