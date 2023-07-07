using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSnake : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] Transform[] TailBones;
  [SerializeField] int TailBoneSeparationTicks = 10;
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

  List<Vector3> TrailPos = new();
  int MaxTrailLength => TailBoneSeparationTicks * (TailBones.Length + 1);

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

    TrailPos.Insert(0, transform.position);
    if (TrailPos.Count > MaxTrailLength)
      TrailPos.RemoveAt(MaxTrailLength);

    var oldPos = transform.position;
    Controller.Move(Time.fixedDeltaTime * Velocity);
    var desired = Quaternion.Euler(0, Angle, 0) * Vector3.forward;
    Controller.Rotation(Quaternion.LookRotation(desired));

    MoveTailBones(transform.position - oldPos);
  }

  void MoveTailBones(Vector3 dp) {
    for (int pi = TailBoneSeparationTicks, bi = 0; pi < TrailPos.Count && bi < TailBones.Length; bi++, pi += TailBoneSeparationTicks) {
      var b = TailBones[bi];
      var p0 = TrailPos[pi];
      var p1 = TrailPos[pi-1];
      b.position = p0;
      if (p0.TryGetDirection(p1, out var dir))
        b.forward = dir;
    }
  }

  //public float DebugTurnAccel = 0;
  //void OnGUI() {
  //  var dir = transform.right * Mathf.Sign(DebugTurnAccel);
  //  GUIExtensions.DrawLine(transform.position, transform.position + dir, 1);
  //}
}