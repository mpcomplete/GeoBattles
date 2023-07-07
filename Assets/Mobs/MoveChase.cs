using UnityEngine;

// Chase the player.
// Used by: Diamond, Redbox+small, Greenbox, Triangle
public class MoveChase : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float Acceleration = 2;
  [SerializeField] float MaxSpeed = 2;

  public Vector3 Velocity;
  Transform Target => GameManager.Instance.Players.Count > 0 ? GameManager.Instance.Players[0].transform : null;
  Vector3 TargetDelta => Target.position - transform.position;

  void Start() {
    Controller.SetMaxMoveSpeed(MaxSpeed);
  }

  void FixedUpdate() {
    if (!Target)
      return;
    var accel = Acceleration * TargetDelta.normalized;
    Velocity += MaxSpeed * Time.fixedDeltaTime * accel;
    if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
      Velocity = MaxSpeed * Velocity.normalized;
    Controller.Move(Time.fixedDeltaTime * Velocity);
  }
}