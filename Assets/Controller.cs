using UnityEngine;

public class Controller : MonoBehaviour {
  public float Radius = .5f;
  public bool IgnoreBounds = false;

  float MaxMoveSpeed;

  [Header("State")]
  public Vector3 ScriptVelocity;
  public Vector3 PhysicsVelocity;
  public Vector3 PhysicsAccel;

  public void SetMaxMoveSpeed(float maxMoveSpeed) => MaxMoveSpeed = maxMoveSpeed;

  public void MoveV(Vector3 velocity) {
    ScriptVelocity += velocity;
  }
  public void Rotation(Quaternion rotation) {
    transform.rotation = rotation;
  }

  public void AddPhysicsVelocity(Vector3 dv) {
    PhysicsVelocity += dv;
  }
  public void DampenVelocity(float factor) {
    PhysicsVelocity *= factor;
  }
  public void AddPhysicsAccel(Vector3 da) {
    PhysicsAccel += da;
  }

  void DoMove(Vector3 delta) {
    if (IgnoreBounds) {
      transform.position += delta;
      return;
    }
    var hit = new BoundHit();
    var didHit = Bounds.Instance.Collide(this, delta, ref hit);
    if (didHit) {
      PhysicsVelocity -= Vector3.Project(PhysicsVelocity, hit.Normal);
      SendMessage("OnBoundsHit", hit, SendMessageOptions.DontRequireReceiver);
      Debug.DrawRay(hit.Point, hit.Normal);
    }
  }

  void FixedUpdate() {
    var steeringVector = ScriptVelocity - PhysicsVelocity;
    var desiredMagnitude = steeringVector.magnitude;
    var maxSteeringMagnitude = 2f * MaxMoveSpeed;
    var boundedSteeringVelocity = Mathf.Min(desiredMagnitude, maxSteeringMagnitude) * steeringVector.normalized;
    PhysicsVelocity += boundedSteeringVelocity;
    PhysicsVelocity += Time.fixedDeltaTime * PhysicsAccel;
    PhysicsAccel = Vector3.zero;
    ScriptVelocity = Vector3.zero;

    DoMove(Time.fixedDeltaTime * PhysicsVelocity);
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, Radius);
  }
}