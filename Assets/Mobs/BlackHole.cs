using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {
  [SerializeField] Character Character;
  [SerializeField] MonoBehaviour[] ActivationAbilities;
  [SerializeField] ParticleSystem OrbitParticles;
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
      ActivationAbilities.ForEach(a => { a.gameObject.SetActive(true); a.enabled = true; });
      BaseForceMagnitude = GridForce.Magnitude;
      //OrbitParticles.Play();
      //OrbitParticles.GetComponent<ParticleSystemForceField>().enabled = true;
    }
  }

  float BaseForceMagnitude;
  void Pulse() {
    var healthFactor = 1f + PulseRateHealthFactor*ExplodePct;
    var t = (Time.fixedTime * PulseRate * healthFactor) % 1f;
    var s = PulseSize.Evaluate(t);
    transform.localScale *= s;
  }

  void PulseGrid() {
    var t = (Time.fixedTime * GridPulseRate) % 1f;
    var s = GridPulseSize.Evaluate(t);
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