using UnityEngine;

public class MovePinwheel : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float Acceleration = 2;
  [SerializeField] float MaxSpeed = 1;
  [SerializeField] float MaxTurnDegrees = 5;
  Vector3 Accel;
  Vector3 Velocity;

  void Start() {
    Controller.SetMaxMoveSpeed(MaxSpeed);
    Accel = Random.onUnitSphere.XZ().normalized * 2f;
  }

  void FixedUpdate() {
    float turnDegrees = Random.Range(-MaxTurnDegrees, MaxTurnDegrees);
    var accelRight = Vector3.Cross(Accel, Vector3.up);
    Accel = Vector3.RotateTowards(Accel, accelRight * (turnDegrees > 0 ? 1 : -1), Mathf.Abs(turnDegrees) * Mathf.Deg2Rad, 0f);
    Velocity += MaxSpeed * Time.fixedDeltaTime * Accel;
    if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
      Velocity = MaxSpeed * Velocity.normalized;
    Controller.Move(Time.fixedDeltaTime * Velocity);
  }

  void OnBoundsHit(BoundHit hit) {
    Accel = Vector3.Reflect(Accel, hit.Normal);
    Velocity = Vector3.Reflect(Velocity, hit.Normal);
  }
}