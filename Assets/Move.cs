using UnityEngine;

public class Move : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;
  [SerializeField] Controller Controller;
  [SerializeField] float Speed = 10;
  [SerializeField] float RotationSpeed = 180;
  [SerializeField] float ShotPeriodSeconds = .25f;

  Vector3 Aim;
  Vector3 Velocity;
  Quaternion Rotation;
  float ShotCooldownSeconds;

  void Start() {
    Time.fixedDeltaTime = 1f/60f;
    InputHandler.OnMove += OnMove;
    Rotation = transform.rotation;
  }

  void FixedUpdate() {
    Controller.Move(Time.deltaTime * Velocity);
    Controller.Rotation(Quaternion.RotateTowards(transform.rotation, Rotation, Time.deltaTime * RotationSpeed));
  }

  void OnMove(Vector3 v) {
    Velocity = Speed * v;
    Rotation = Quaternion.LookRotation(v.sqrMagnitude > 0 ? v.normalized : transform.forward);
  }
}