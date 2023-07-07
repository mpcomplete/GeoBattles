using UnityEngine;

public class Move : MonoBehaviour {
  [SerializeField] InputHandler InputHandler;
  [SerializeField] Controller Controller;
  [SerializeField] float Speed = 10;
  [SerializeField] float RotationSpeed = 180;
  [SerializeField] ParticleSystem ThrusterParticles;

  Vector3 Velocity;
  Quaternion Rotation;

  void Start() {
    Controller.SetMaxMoveSpeed(Speed);
    InputHandler.OnMove += OnMove;
    Rotation = transform.rotation;
  }

  void FixedUpdate() {
    Controller.Move(Time.deltaTime * Velocity);
    Controller.Rotation(Quaternion.RotateTowards(transform.rotation, Rotation, Time.deltaTime * RotationSpeed));
    if (Velocity.sqrMagnitude > 0 && !ThrusterParticles.isPlaying) {
      ThrusterParticles.Play();
    } else if (Velocity.sqrMagnitude <= 0 && ThrusterParticles.isPlaying) {
      ThrusterParticles.Stop();
    }
  }

  void OnMove(Vector3 v) {
    Velocity = Speed * v;
    Rotation = Quaternion.LookRotation(v.sqrMagnitude > 0 ? v.normalized : transform.forward);
  }
}