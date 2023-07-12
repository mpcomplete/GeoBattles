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

  Vector3 Velocity;
  public override void Suck(Vector3 accel) {
    var a = GravityVulnerability * accel;
    Velocity += Time.fixedDeltaTime * a;
  }

  private void FixedUpdate() {
    transform.position += Time.fixedDeltaTime * Velocity;
    Velocity *= VelocityDamping;
  }

  void Awake() {
    this.InitComponentFromParent(out Tail);
  }
}