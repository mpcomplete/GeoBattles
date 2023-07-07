using UnityEngine;

public class MoveRedboxCircle : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float CircleMoveSpeed = 2;
  [SerializeField] float CircleTurnSpeed = 2;
  [SerializeField] float Angle = 0f;

  Vector3 Accel;
  Vector3 Velocity;

  void Start() {
    Controller.SetMaxMoveSpeed(CircleMoveSpeed);
  }

  void FixedUpdate() {
    Angle += CircleTurnSpeed * Time.fixedDeltaTime;
    if (Angle > 360f) Angle = 0f;
    var tangent = new Vector3(Mathf.Cos(Angle), 0f, Mathf.Sin(Angle));
    Accel = tangent;
    //Velocity += CircleMoveSpeed * Time.deltaTime * Accel;
    Velocity = CircleMoveSpeed * Accel;
    if (Velocity.sqrMagnitude > CircleMoveSpeed.Sqr())
      Velocity = CircleMoveSpeed * Velocity.normalized;
    Controller.Move(Time.deltaTime * Velocity);
  }
}