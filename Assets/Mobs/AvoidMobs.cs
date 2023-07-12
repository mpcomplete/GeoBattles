using UnityEngine;

public class AvoidMobs : MonoBehaviour {
  [SerializeField] Controller Controller;
  //[SerializeField] float Acceleration = 2;
  //[SerializeField] float MaxSpeed = 2;
  [SerializeField] float SeparationDistance = 2f;
  [SerializeField] float SeparationStrength = 1f;
  [SerializeField] float SeparationTangentStrength = .2f;

  // Only process 1/NumBuckets mobs per frame.
  int BucketIndex = 0;
  static int BucketCount = 0;
  static int NumBuckets = 10;

  void Start() {
    BucketIndex = BucketCount;
    BucketCount = (BucketCount + 1) % NumBuckets;
  }

  void FixedUpdate() {
    //if (Timeval.TickCount % NumBuckets != BucketIndex) return;
    //var others = GameManager.Instance.Mobs;
    //var nearAvg = Vector3.zero;
    //foreach (var mob in others) {
    //  var delta = transform.position - mob.transform.position;
    //  if (delta.sqrMagnitude < SeparationDistance.Sqr())
    //    nearAvg += delta;
    //}
    //nearAvg = nearAvg.normalized;
    var nearAvg = AvoidMobsManager.Instance.GetAvoidForce(transform.position);
    var tangent = Vector3.Cross(nearAvg, Vector3.up);
    var velocity = SeparationStrength*nearAvg + SeparationTangentStrength*tangent;
    //if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
    //  Velocity = MaxSpeed * Velocity.normalized;
    //Controller.Move(Time.fixedDeltaTime * velocity);
    Controller.AddPhysicsVelocity(velocity);
  }
}