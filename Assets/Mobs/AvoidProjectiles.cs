using UnityEngine;

public class AvoidProjectiles : MonoBehaviour {
  [SerializeField] Controller Controller;
  [SerializeField] float AvoidDistance = 2f;
  [SerializeField] float AvoidStrength = 5f;
  [SerializeField] float MaxSpeed = 5f;
  [SerializeField] float VelocityDampening = .9f;

  Vector3 Velocity;
  void FixedUpdate() {
    var avgForce = Vector3.zero;
    foreach (var p in GameManager.Instance.Projectiles) {
      var delta = transform.position - p.transform.position;
      var pv = p.GetComponent<Rigidbody>().velocity.normalized;
      var headingTowards = Vector3.Dot(delta, pv) > 0;
      if (headingTowards && delta.sqrMagnitude < AvoidDistance.Sqr()) {
        var perpendicular = delta.normalized - pv;  // the further perpendicular to projectile's trajectory
        if (perpendicular.sqrMagnitude < .1f.Sqr())  // If the bullet is heading right at us, just dodge left.
          perpendicular = Vector3.Cross(pv, Vector3.up);
        var avoidDir = perpendicular.normalized + pv;
        avgForce += avoidDir.normalized * (AvoidDistance - delta.magnitude);
      }
    }

    var accel = AvoidStrength * avgForce;
    Velocity *= VelocityDampening;
    Velocity += MaxSpeed * Time.fixedDeltaTime * accel;
    if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
      Velocity = MaxSpeed * Velocity.normalized;
    Controller.Move(Time.fixedDeltaTime * Velocity);

    DebugAvgForce = avgForce;
  }

  Vector3 DebugAvgForce;
  //void OnGUI() {
  //  if (DebugAvgForce.sqrMagnitude > 0f)
  //    GUIExtensions.DrawLine(transform.position, transform.position + DebugAvgForce, 1);
  //}
}