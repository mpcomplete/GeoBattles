using System.Linq;
using UnityEngine;

public class MoveDiamond : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float Acceleration = 2;
  [SerializeField] float MaxSpeed = 2;

  Transform Target;
  Vector3 Accel;
  Vector3 Velocity;
  Vector3 TargetDelta => Target.position - transform.position;

  void Start() {
    Target = FindObjectOfType<Player>().transform;
  }

  void FixedUpdate() {
    Accel = Acceleration * TargetDelta.normalized;
    Velocity += MaxSpeed * Time.deltaTime * Accel;
    if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
      Velocity = MaxSpeed * Velocity.normalized;
    Controller.Move(Time.deltaTime * Velocity);
  }
}