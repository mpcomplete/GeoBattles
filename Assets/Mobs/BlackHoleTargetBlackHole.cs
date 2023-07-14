using UnityEngine;

public class BlackHoleTargetBlackHole : BlackHoleTarget {
  public BlackHole BlackHole;
  public float SeparationDistance = 2f;
  public float TangentStrength = 2f;

  public override void Suck(BlackHole hole, float gravity, Vector3 toHole) {
    base.Suck(hole, gravity, toHole);
    if (BlackHole.Activated && toHole.sqrMagnitude < SeparationDistance.Sqr()) {
      // Wacky hacky!
      var tangent = Vector3.Cross(toHole.normalized, Vector3.up);
      Controller.PhysicsAccel = Vector3.RotateTowards(Controller.PhysicsAccel, tangent, TangentStrength * Mathf.Deg2Rad, 0f);
    }
  }
}