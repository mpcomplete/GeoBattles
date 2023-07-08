using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShipManager : MonoBehaviour {
  public static ShipManager Instance;

  [SerializeField] Player PlayerPrefab;
  [SerializeField] int BonusShipScoreInterval = 75000;
  [SerializeField] int ShipCount = 3;

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
    Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
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
    if (ShipCount <= 0) {
      GameManager.Instance.LevelEnd?.Invoke();
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