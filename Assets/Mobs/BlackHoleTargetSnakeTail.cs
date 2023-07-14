using UnityEngine;

public class BlackHoleTargetSnakeTail : BlackHoleTarget {
  public MoveSnakeTail Tail;
  public int Index;  // Initialized by MoveSnakeTail.
  public float VelocityDamping = .9f;

  public override void OnEaten(BlackHole hole) {
    if (Tail) {
      Tail.OnTailEaten(hole, Index);
      gameObject.SetActive(false);
    } else {
      Destroy(gameObject);
    }
  }

  void Awake() {
    this.InitComponentFromParent(out Tail);
  }
}