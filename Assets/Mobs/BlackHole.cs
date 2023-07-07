using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: deform grid
public class BlackHole : MonoBehaviour {
  [SerializeField] Character Character;
  [SerializeField] ParticleSystem ActivationParticles;
  [SerializeField] float PulseRate = 5f;
  [SerializeField] float PulseRateHealthFactor = 1.5f;
  [SerializeField] AnimationCurve PulseSize;
  [SerializeField] AnimationCurve HealthToSize;
  [SerializeField] float Gravity = 125f;
  [SerializeField] int ExplodeHealth = 15;
  [SerializeField] float MaxDistance = 5f;
  [SerializeField] float EatDistance = 1f;
  [SerializeField] GameObject DeathSpawn;

  bool Activated = false;
  IEnumerable<Controller> Bodies => GameManager.Instance.Mobs.Concat(GameManager.Instance.Players).Where(c => c != Character).Select(c => c.GetComponent<Controller>());
  float ExplodePct => (float)Character.Health / ExplodeHealth;

  void OnHurt() {
    if (!Activated) {
      Activated = true;
      Instantiate(ActivationParticles, transform.position, transform.rotation);
    }
  }

  float PulseT = 0f;
  void Pulse() {
    var healthFactor = 1f + PulseRateHealthFactor*ExplodePct;
    PulseT += Time.fixedDeltaTime * PulseRate * healthFactor;
    if (PulseT > 1f) PulseT = 0f;
    var s = PulseSize.Evaluate(PulseT);
    transform.localScale *= s;
  }

  void Suck() {
    List<Controller> eaten = new();
    foreach (var b in Bodies) {
      var toHole = transform.position - b.transform.position;
      if (toHole.sqrMagnitude < MaxDistance.Sqr()) {
        var r = toHole.magnitude;
        b.AddPhysicsAccel(b.GravityVulnerability * Gravity  / (r*r) * toHole.normalized);  // g/r^2
      }
      if (toHole.sqrMagnitude < EatDistance.Sqr()) {
        eaten.Add(b);
      }
    }
    foreach (var b in eaten) {
      Eat(b);
    }
  }

  // Called when mob/player collides with us.
  void Eat(Controller c) {
    if (c.TryGetComponent(out Player p)) {
      p.Kill();
    } else if (c.TryGetComponent(out Mob m)) {
      Destroy(m.gameObject);
      Character.Health += 1;
      GetComponent<Mob>().BaseScore += 100;
      if (Character.Health >= ExplodeHealth)
        Explode();
    }
  }

  [ContextMenu("Explode")]
  void Explode() {
    // TODO: Particles?
    Instantiate(DeathSpawn, transform.position, Quaternion.identity);
    Destroy(gameObject);
  }

  void FixedUpdate() {
    if (Activated) {
      var s = HealthToSize.Evaluate(ExplodePct);
      transform.localScale = new(s, s, s);
      Pulse();
      Suck();
    }
  }
}