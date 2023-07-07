using System;
using System.Collections;
using UnityEngine;

// Windup and charge the player.
// Used by: Nemesis
public class MoveCharge : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float WindupTurnSpeed = 45f;
  [SerializeField] float WindupSeconds = .2f;
  [SerializeField] float ChargeDistance = 10f;
  [SerializeField] float ChargeTurnSpeed = 25f;
  [SerializeField] float Acceleration = 5f;
  [SerializeField] float MaxSpeed = 10f;
  [SerializeField] float CooldownSeconds = 1f;
  [SerializeField] float VelocityDampening = .9f;

  public Vector3 Velocity;
  Transform Target;
  Vector3 TargetDelta => Target.position - transform.position;

  void Start() {
    Controller.SetMaxMoveSpeed(MaxSpeed);
    Target = FindObjectOfType<Player>().transform;
    StartCoroutine(WaitAndCharge());
  }

  bool Waiting = false;
  IEnumerator WaitAndCharge() {
    Waiting = true;
    var facePlayer = StartCoroutine(FacePlayer());
    yield return new WaitForSeconds(WindupSeconds);
    Waiting = false;
    yield return facePlayer;
    yield return Charge();
    yield return new WaitForSeconds(CooldownSeconds);
    StartCoroutine(WaitAndCharge());
  }

  const float Threshold = .1f;
  IEnumerator FacePlayer() {
    while (Waiting || (transform.forward - TargetDelta.normalized).sqrMagnitude > Threshold.Sqr()) {
      Controller.Rotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(TargetDelta.normalized), Time.deltaTime * WindupTurnSpeed));
      yield return new WaitForFixedUpdate();
    }
  }

  bool Charging = false;
  IEnumerator Charge() {
    SendMessage("OnCharge", SendMessageOptions.RequireReceiver);
    Charging = true;
    Velocity = Vector3.zero;
    var endTime = ChargeDistance / MaxSpeed;
    for (var t = 0f; t < endTime; t += Time.fixedDeltaTime) {
      var accel = Acceleration * transform.forward;
      Velocity += MaxSpeed * Time.deltaTime * accel;
      Controller.Rotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(TargetDelta.normalized), Time.deltaTime * ChargeTurnSpeed));
      yield return new WaitForFixedUpdate();
    }
    Charging = false;
  }

  void FixedUpdate() {
    if (Velocity.sqrMagnitude > 0f) {
      if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
        Velocity = MaxSpeed * Velocity.normalized;
      if (!Charging)
        Velocity *= VelocityDampening;
      var dx = Time.deltaTime * Velocity;
      Controller.Move(dx);
    }
  }
}