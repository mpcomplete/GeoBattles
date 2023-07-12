using UnityEngine;

public class AvoidProjectiles : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float AvoidDistance = 2f;
  [SerializeField] float AvoidStrength = 5f;
  [SerializeField] float TangentBias = .25f;
  [SerializeField] float VelocityDampening = .9f;
  [SerializeField] float MaxSpeed = 15f;

  void FixedUpdate() {
    var avgForce = Vector3.zero;
    foreach (var p in GameManager.Instance.Projectiles) {
      var toMe = transform.position - p.transform.position;
      var pv = p.GetComponent<Rigidbody>().velocity.normalized;
      var headingTowards = Vector3.Dot(toMe, pv) > 0;
      if (headingTowards && toMe.sqrMagnitude < AvoidDistance.Sqr()) {
        var perpendicular = toMe.normalized - pv;  // the further perpendicular to projectile's trajectory
        if (perpendicular.sqrMagnitude < .1f.Sqr())  // If the bullet is heading right at us, just dodge left.
          perpendicular = Vector3.Cross(pv, Vector3.up);
        var avoidDir = Vector3.Lerp(pv, perpendicular.normalized, TangentBias);
        avgForce += (1f - Mathf.Pow(toMe.magnitude/AvoidDistance, 2f)) * avoidDir.normalized;
        //avgForce += (1f - toMe.magnitude/AvoidDistance) * avoidDir.normalized;
      }
    }

    var accel = AvoidStrength * avgForce.normalized;
    Controller.DampenVelocity(VelocityDampening);
    Controller.AddPhysicsVelocity(MaxSpeed * Time.fixedDeltaTime * accel);
    //if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
    //  Velocity = MaxSpeed * Velocity.normalized;
    //Controller.Move(Time.fixedDeltaTime * Velocity);

    DebugAvgForce = avgForce;
  }

  Vector3 DebugAvgForce;
  //void OnGUI() {
  //  if (DebugAvgForce.sqrMagnitude > 0f)
  //    GUIExtensions.DrawLine(transform.position, transform.position + DebugAvgForce, 1);
  //}
}