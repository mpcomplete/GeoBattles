using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSnake : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] Transform[] TailBones;
  [SerializeField] float TailBoneSeparationDist = 1f;
  [SerializeField] float TurnAcceleration = 5f;
  [SerializeField] float MaxTurnSpeed = 25f;
  [SerializeField] float Acceleration = 5f;
  [SerializeField] float MaxSpeed = 10f;

  public float Speed = 0f;
  public Vector3 Velocity;
  public float Angle = 0f;
  public float TurnSpeed = 0;
  Transform Target;
  Vector3 TargetDelta => Target.position - transform.position;

  void Start() {
    Controller.SetMaxMoveSpeed(MaxSpeed);
    Target = FindObjectOfType<Player>().transform;
    Angle = transform.rotation.eulerAngles.y;
    TurnSpeed = 0;
    TailBones[0].parent.SetParent(null);  // Detach the tailbone root so they move separately.
  }

  void FixedUpdate() {
    var targetAngle = Vector3.SignedAngle(Vector3.forward, TargetDelta, Vector3.up);
    var diff = Mathf.DeltaAngle(Angle, targetAngle);
    var turnAccel = TurnAcceleration * Mathf.Sign(diff);
    TurnSpeed += Time.fixedDeltaTime * turnAccel;
    TurnSpeed = Mathf.Clamp(TurnSpeed, -MaxTurnSpeed, MaxTurnSpeed);
    Angle += Time.fixedDeltaTime * TurnSpeed;

    Speed += Time.fixedDeltaTime * Acceleration;
    Speed = Mathf.Min(Speed, MaxSpeed);
    Velocity = Speed * transform.forward;

    Controller.Move(Time.fixedDeltaTime * Velocity);
    var desired = Quaternion.Euler(0, Angle, 0) * Vector3.forward;
    Controller.Rotation(Quaternion.LookRotation(desired));

    MoveTailBones();
  }

  void MoveTailBones() {
    for (int i = TailBones.Length - 1; i >= 0; i--) {
      var tb = TailBones[i].transform;
      var tbNext = i > 0 ? TailBones[i-1].transform : transform;

      var oldPos = tb.position;
      tb.position = Vector3.Slerp(tb.position, tbNext.position, Time.fixedDeltaTime / TailBoneSeparationDist);
      if ((tb.position - oldPos).sqrMagnitude > Mathf.Epsilon)
        tb.forward = tb.position - oldPos;
    }
  }

  //public float DebugTurnAccel = 0;
  //void OnGUI() {
  //  var dir = transform.right * Mathf.Sign(DebugTurnAccel);
  //  GUIExtensions.DrawLine(transform.position, transform.position + dir, 1);
  //}
}