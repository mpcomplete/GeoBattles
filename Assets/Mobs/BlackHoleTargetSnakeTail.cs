public class BlackHoleTargetSnakeTail : BlackHoleTarget {
  public MoveSnakeTail Tail;
  public int Index;  // Initialized by MoveSnakeTail.

  public override void OnEaten(BlackHole hole) {
    if (Tail) {
      Tail.OnTailEaten(Index);
      gameObject.SetActive(false);
    } else {
      Destroy(gameObject);
    }
  }

  public void OnOtherEaten(int index) {
  }

  void Awake() {
    this.InitComponentFromParent(out Tail);
  }
}