using System;
using UnityEngine;

// Identifier for which team a character is on - e.g., player or mob.
public class Shoot : MonoBehaviour {
  [Serializable]
  public struct Variant {
    public GameObject Projectile;
    public float AttacksPerSecond;
  }
  public Variant[] ProjectileVariants;
  public int CurrentVariant = 0;
  //public InputAction Axis;

  static GameObject _Parent;
  static GameObject Parent => _Parent = _Parent ? _Parent : new GameObject("Projectiles");

  public void Spawn(Variant variant, Vector3 dir) {
    var p = Instantiate(variant.Projectile, transform.position, Quaternion.LookRotation(dir));
    p.transform.SetParent(Parent.transform, true);
  }

  int CooldownTicksRemaining = 0;
  public void FixedUpdate() {
    if (CooldownTicksRemaining > 0) {
      CooldownTicksRemaining--;
    } else {
      var variant = ProjectileVariants[CurrentVariant];
      var axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
      if (axis.sqrMagnitude > 0.1f.Sqr()) {
        CooldownTicksRemaining = Timeval.FromSeconds(1f / variant.AttacksPerSecond).Ticks;
        Spawn(variant, axis.XZ());
      }
    }
  }

  private void Awake() {
    Time.fixedDeltaTime = 1f / Timeval.FixedUpdatePerSecond;
  }
}