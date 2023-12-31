using System;
using UnityEngine;

[Serializable]
public struct Variant {
  public GameObject Projectile;
  public float AttacksPerSecond;
  public AudioSource AudioSource;
}

public class ShotManager : LevelManager<ShotManager> {
  [SerializeField] int ShotChangeScoreInterval = 1000;
  [SerializeField] Variant DefaultShotVariant;
  [SerializeField] Variant[] ShotVariants;

  public int DebugStartVariant = -1;
  public Variant ActiveShotVariant { get; private set; }

  protected override void Awake() {
    base.Awake();
    GameManager.Instance.StartGame += SetDefaultShot;
  }

  void Start() {
    if (DebugStartVariant >= 0)
      ActiveShotVariant = ShotVariants[DebugStartVariant];
    ScoreManager.Instance.ScoreChange += TryToggleActiveShot;
  }

  void OnDestroy() {
    GameManager.Instance.StartGame -= SetDefaultShot;
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

  public void SetShotVariant(int idx) {
    ActiveShotVariant = idx >= ShotVariants.Length ? DefaultShotVariant : ShotVariants[idx];
  }

  [ContextMenu("SetVariant1")]
  void SetVariant1() => ActiveShotVariant = ShotVariants[0];
  [ContextMenu("SetVariant2")]
  void SetVariant2() => ActiveShotVariant = ShotVariants[1];
}
