using System;
using UnityEngine;

[Serializable]
public struct Variant {
  public GameObject Projectile;
  public float AttacksPerSecond;
}

public class ShotManager : MonoBehaviour {
  public static ShotManager Instance;

  [SerializeField] int ShotChangeScoreInterval = 1000;
  [SerializeField] Variant DefaultShotVariant;
  [SerializeField] Variant[] ShotVariants;

  public Variant ActiveShotVariant { get; private set; }

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
      GameManager.Instance.LevelStart += SetDefaultShot;
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    ScoreManager.Instance.ScoreChange += TryToggleActiveShot;
  }

  void OnDestroy() {
    GameManager.Instance.LevelStart -= SetDefaultShot;
    ScoreManager.Instance.ScoreChange -= TryToggleActiveShot;
  }

  void SetDefaultShot() {
    ActiveShotVariant = DefaultShotVariant;
  }

  void TryToggleActiveShot(ScoreEvent scoreEvent) {
    var previousScore = scoreEvent.ScoreTotal-scoreEvent.ScoreChange;
    var newScore = scoreEvent.ScoreTotal;
    if ((previousScore / ShotChangeScoreInterval) < (newScore / ShotChangeScoreInterval)) {
      ActiveShotVariant = ShotVariants.Random();
    }
  }
}