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
    GetComponent<Player>().OnDying += KillParticles;
    Controller.SetMaxMoveSpeed(Speed);
    InputHandler.OnMove += OnMove;
    Rotation = transform.rotation;
  }

  void OnDestroy() {
    GetComponent<Player>().OnDying -= KillParticles;
  }

  void KillParticles() {
    ThrusterParticles.Clear();
    ThrusterParticles.Stop();
  }

  void FixedUpdate() {
    Controller.Move(Time.fixedDeltaTime * Velocity);
    Controller.Rotation(Quaternion.RotateTowards(transform.rotation, Rotation, Time.fixedDeltaTime * RotationSpeed));
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