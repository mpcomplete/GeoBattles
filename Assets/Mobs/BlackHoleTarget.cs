using UnityEngine;

public class BlackHoleTarget : MonoBehaviour {
  public Controller Controller;
  public Character Character;
  public float GravityVulnerability = 1f;

  public virtual void OnEaten(BlackHole hole) {
    if (Character is Mob m)
      m.Despawn();
    enabled = false;
  }

  public virtual void Suck(BlackHole hole, float gravity, Vector3 toHole) {
    var r = toHole.magnitude;
    toHole /= r; // normalized
    var accel = GravityVulnerability * gravity / (r*r) * toHole;  // g/r^2
    var newV = Controller.PhysicsVelocity + Time.fixedDeltaTime*accel;
    var newP = transform.position + Time.fixedDeltaTime*newV;
    if (Vector3.Dot(hole.transform.position - newP, toHole) < 0) { // overshot, just land in hole
      transform.position = hole.transform.position;
      return;
    }
    Controller.AddPhysicsAccel(accel);
  }

  void OnEnable() => GameManager.Instance.BlackHoleTargets.Add(this);
  void OnDisable() => GameManager.Instance.BlackHoleTargets.Remove(this);
}