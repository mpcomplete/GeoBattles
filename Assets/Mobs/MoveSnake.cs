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
  Transform TailRoot;
  int TailEatenIndex = -1;

  void Start() {
    Controller.SetMaxMoveSpeed(MaxSpeed);
    Target = FindObjectOfType<Player>().transform;
    Angle = transform.rotation.eulerAngles.y;
    TurnSpeed = 0;
    TailRoot = TailBones[0].parent;
    TailRoot.SetParent(null);  // Detach the tailbone root so they move separately.
    for (var i = 0; i < TailBones.Length; i++) {
      if (TailBones[i].TryGetComponent(out BlackHoleTargetSnakeTail b)) {
        b.SnakeHead = this;
        b.Index = i;
      }
    }
  }

  void OnDestroy() {
    TailRoot.DetachChildren();
    Destroy(TailRoot.gameObject);
    for (var i = 0; i < TailBones.Length; i++) {
      if (TailBones[i].TryGetComponent(out BlackHoleTargetSnakeTail b)) {
        b.OnHeadDestroyed();
      } else { // already got eaten
        Destroy(TailBones[i].gameObject);
      }
    }
  }

  void FixedUpdate() {
    if (TailEatenIndex >= 0) {
      SuckToTail();
    }

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
    for (int i = TailBones.Length - 1; i >= 0; i--)
      MoveTailBone(TailBones[i].transform, i > 0 ? TailBones[i-1].transform : transform, false);
  }

  void SuckToTail() {
    for (int i = TailBones.Length-1; i > TailEatenIndex; i--)
      MoveTailBone(TailBones[i].transform, i > 0 ? TailBones[i-1].transform : transform, false);
    for (int i = TailEatenIndex; i >= 0; i--)
      MoveTailBone(i > 0 ? TailBones[i-1].transform : transform, TailBones[i].transform, true);
  }

  void MoveTailBone(Transform tb, Transform tbNext, bool reversed) {
    var oldPos = tb.position;
    tb.position = Vector3.Slerp(tb.position, tbNext.position, Time.fixedDeltaTime / TailBoneSeparationDist);
    var forward = tb.position - oldPos;
    if (forward.sqrMagnitude > .001f)
      tb.forward = reversed ? -forward : forward;
  }

  public void OnTailEaten(int index) {
    if (TailEatenIndex < 0) TailEatenIndex = index;
    foreach (var tb in TailBones) {
      tb.GetComponent<BlackHoleTargetSnakeTail>().enabled = false;
    }
  }

  //public float DebugTurnAccel = 0;
  //void OnGUI() {
  //  var dir = transform.right * Mathf.Sign(DebugTurnAccel);
  //  GUIExtensions.DrawLine(transform.position, transform.position + dir, 1);
  //}
}