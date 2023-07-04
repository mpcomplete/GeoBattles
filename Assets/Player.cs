using UnityEngine;

public class Player : MonoBehaviour {
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
    Debug.LogWarning("You are setting fixed framerate in Player");
    Time.fixedDeltaTime = 1f/60f;
    InputHandler.OnAim += OnAim;
    InputHandler.OnMove += OnMove;
    Rotation = transform.rotation;
  }

  void FixedUpdate() {
    Controller.Move(Time.deltaTime * Velocity);
    Controller.Rotation(Quaternion.RotateTowards(transform.rotation, Rotation, Time.deltaTime * RotationSpeed));
    ShotCooldownSeconds -= Time.deltaTime;
    if (ShotCooldownSeconds <= 0 && Aim.sqrMagnitude > 0) {
      Fire();
    }
  }

  void OnMove(Vector3 v) {
    Velocity = Speed * v;
    Rotation = Quaternion.LookRotation(v.sqrMagnitude > 0 ? v.normalized : transform.forward);
  }

  void OnAim(Vector3 v) {
    Aim = v;
  }

  void Fire() {
    // Steve: This "makes sense" to me but I'm not sure it captures the correct logic
    ShotCooldownSeconds = (ShotCooldownSeconds % ShotPeriodSeconds) + ShotPeriodSeconds;
  }
}