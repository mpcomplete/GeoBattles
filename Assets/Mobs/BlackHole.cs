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
  [SerializeField] float GridPulseRate = .5f;
  [SerializeField] AnimationCurve GridPulseSize;
  [SerializeField] AnimationCurve HealthToSize;
  [SerializeField] float Gravity = 125f;
  [SerializeField] int ExplodeHealth = 15;
  [SerializeField] float MaxDistance = 5f;
  [SerializeField] float EatDistance = 1f;
  [SerializeField] GameObject DeathSpawn;
  [SerializeField] GridForce GridForce;

  bool Activated = false;
  float ExplodePct => (float)Character.Health / ExplodeHealth;

  void OnHurt() {
    if (!Activated) {
      Activated = true;
      GridForce.enabled = true;
      BaseForceMagnitude = GridForce.Magnitude;
      Instantiate(ActivationParticles, transform.position, transform.rotation);
    }
  }

  float PulseT = 0f;
  float BaseForceMagnitude;
  void Pulse() {
    var healthFactor = 1f + PulseRateHealthFactor*ExplodePct;
    PulseT += Time.fixedDeltaTime * PulseRate * healthFactor;
    if (PulseT > 1f) PulseT = 0f;
    var s = PulseSize.Evaluate(PulseT);
    transform.localScale *= s;
  }

  float GridPulseT = 0f;
  void PulseGrid() {
    GridPulseT += Time.fixedDeltaTime * GridPulseRate;
    if (GridPulseT > 1f) GridPulseT = 0f;
    var s = GridPulseSize.Evaluate(GridPulseT);
    GridForce.Magnitude = BaseForceMagnitude * s;
  }

  void Suck() {
    List<BlackHoleTarget> eaten = new();
    foreach (var b in GameManager.Instance.BlackHoleTargets) {
      if (b.gameObject == gameObject) continue;

      var toHole = transform.position - b.transform.position;
      if (toHole.sqrMagnitude < MaxDistance.Sqr()) {
        var r = toHole.magnitude;
        b.Suck(Gravity  / (r*r) * toHole.normalized);  // g/r^2
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
  void Eat(BlackHoleTarget c) {
    c.OnEaten(this);
    if (c.TryGetComponent(out Mob m)) {
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
      PulseGrid();
      Suck();
    }
  }
}