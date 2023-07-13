using UnityEngine;

public class DeflectProjectiles : MonoBehaviour {
  [SerializeField] float Radius = 5f;
  [SerializeField] float Strength = 5f;

  void FixedUpdate() {
    foreach (var p in GameManager.Instance.Projectiles) {
      var toMe = transform.position - p.transform.position;
      var pv = p.Rigidbody.velocity.normalized;
      var headingTowards = Vector3.Dot(toMe, pv) > 0;
      if (headingTowards && toMe.sqrMagnitude < Radius.Sqr()) {
        var perpendicular = pv - toMe.normalized;  // the further perpendicular to projectile's trajectory
        if (perpendicular.sqrMagnitude < .1f.Sqr()) {  // If the bullet is heading right at us, just bend left.
          perpendicular = Vector3.Cross(pv, Vector3.up);
        } else {
          perpendicular = perpendicular.normalized;
        }
        p.Rigidbody.velocity = Vector3.RotateTowards(p.Rigidbody.velocity, perpendicular, Strength * Mathf.Deg2Rad, 0f);
        p.transform.forward = p.Rigidbody.velocity.normalized;
      }
    }
  }
}