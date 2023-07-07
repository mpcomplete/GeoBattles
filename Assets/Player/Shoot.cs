using UnityEngine;

public class Shoot : MonoBehaviour {
  static GameObject _Parent;
  static GameObject Parent => _Parent = _Parent ? _Parent : new GameObject("Projectiles");

  [SerializeField] InputHandler InputHandler;

  public Variant[] ProjectileVariants;
  public int CurrentVariant = 0;
  Vector3 Aim;
  int CooldownTicksRemaining = 0;

  void Spawn(Variant variant, Vector3 dir) {
    var p = Instantiate(variant.Projectile, transform.position, Quaternion.LookRotation(dir));
    p.transform.SetParent(Parent.transform, true);
  }

  void Start() {
    InputHandler.OnAim += OnAim;
  }

  void OnAim(Vector3 v) {
    Aim = v;
  }

  public void FixedUpdate() {
    if (CooldownTicksRemaining > 0) {
      CooldownTicksRemaining--;
    } else {
      var variant = ShotManager.Instance.ActiveShotVariant;
      if (Aim.sqrMagnitude > 0) {
        CooldownTicksRemaining = Timeval.FromSeconds(1f / variant.AttacksPerSecond).Ticks;
        Spawn(variant, Aim.XZ());
      }
    }
  }
}