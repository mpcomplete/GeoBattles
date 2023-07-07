using UnityEngine;

// Chase the player.
// Used by: Diamond, Redbox+small, Greenbox, Triangle
public class MoveChase : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float Acceleration = 2;
  [SerializeField] float MaxSpeed = 2;

  public Vector3 Velocity;
  Transform Target;
  Vector3 TargetDelta => Target.position - transform.position;

  void Start() {
    Target = FindObjectOfType<Player>().transform;
    Controller.SetMaxMoveSpeed(MaxSpeed);
  }

  void FixedUpdate() {
    var accel = Acceleration * TargetDelta.normalized;
    Velocity += MaxSpeed * Time.deltaTime * accel;
    if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
      Velocity = MaxSpeed * Velocity.normalized;
    Controller.Move(Time.deltaTime * Velocity);
  }
}