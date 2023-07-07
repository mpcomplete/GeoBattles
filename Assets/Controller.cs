using UnityEngine;

public class Controller : MonoBehaviour {
  public float Radius = .5f;

  float MaxMoveSpeed;
  Vector3 ScriptMove = Vector3.zero;

  [Header("Physics")]
  public float PhysicsVelocityDampening = .9f;
  Vector3 PhysicsAccel;
  public Vector3 PhysicsVelocity;

  public void SetMaxMoveSpeed(float maxMoveSpeed) => MaxMoveSpeed = maxMoveSpeed;

  public void Move(Vector3 delta) {
    ScriptMove += delta;
  }

  public void Rotation(Quaternion rotation) {
    transform.rotation = rotation;
  }

  public void AddPhysicsVelocity(Vector3 dv) {
    PhysicsVelocity += dv;
  }
  public void AddPhysicsAccel(Vector3 da) {
    PhysicsAccel += da;
  }

  void DoMove(Vector3 delta) {
    var hit = new BoundHit();
    var didHit = Bounds.Instance.Collide(this, delta, ref hit);
    if (didHit) {
      PhysicsVelocity -= Vector3.Project(PhysicsVelocity, hit.Normal);
      SendMessage("OnBoundsHit", hit, SendMessageOptions.DontRequireReceiver);
      Debug.DrawRay(hit.Point, hit.Normal);
    }
  }

  bool ScriptMovedLastFrame = false;
  void FixedUpdate() {
    if (PhysicsAccel.sqrMagnitude == 0)
      PhysicsVelocity *= PhysicsVelocityDampening;

    if (ScriptMove.sqrMagnitude > 0f) {
      var scriptV = ScriptMove / Time.fixedDeltaTime;
      var nextV = PhysicsVelocity + scriptV;
      // Only physics can increase our speed beyond max.
      if (nextV.sqrMagnitude > MaxMoveSpeed)
        nextV = nextV.normalized * Mathf.Max(PhysicsVelocity.magnitude, MaxMoveSpeed);
      PhysicsVelocity = nextV;
    } else if (ScriptMove.sqrMagnitude == 0 && ScriptMovedLastFrame) {
      // We stopped moving this frame. Apply a breaking force up to our max move speed.
      PhysicsVelocity -= PhysicsVelocity.normalized * Mathf.Min(PhysicsVelocity.magnitude, MaxMoveSpeed);
    }
    PhysicsVelocity += Time.fixedDeltaTime * PhysicsAccel;
    DoMove(Time.fixedDeltaTime * PhysicsVelocity);

    ScriptMovedLastFrame = ScriptMove.sqrMagnitude > 0f;
    ScriptMove = Vector3.zero;
    PhysicsAccel = Vector3.zero;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, Radius);
  }
}