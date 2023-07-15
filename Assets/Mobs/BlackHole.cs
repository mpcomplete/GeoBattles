using System;
using System.Collections.Generic;
using System.Linq;
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
  [SerializeField] [ColorUsage(true, true)] Color GlowColor;
  [SerializeField] AnimationCurve GlowCurve;
  [SerializeField] float MaxDistance = 5f;
  [SerializeField] float EatDistance = 1f;
  [SerializeField] float ProjectileBurstDistance = 3f;
  [SerializeField] float ProjectileBurstForce = 10f;
  [SerializeField] GameObject DeathSpawn;
  [SerializeField] GridForce GridForce;

  public bool Activated = false;
  float ExplodePct => (float)Character.Health / ExplodeHealth;

  void OnHurt() {
    SetGlowColor();

    if (!Activated) {
      Activated = true;
      ActivationAbilities.ForEach(a => { a.gameObject.SetActive(true); a.enabled = true; });
      BaseForceMagnitude = GridForce.Magnitude;
      //OrbitParticles.Play();
      //OrbitParticles.GetComponent<ParticleSystemForceField>().enabled = true;
    } else {
      // Apply an outwards burst force on orbiting blackhole systems when one gets hit.
      var activatedHoles = GameManager.Instance.Mobs.Select(m => m.GetComponent<BlackHole>()).Where(b => b != null && b.Activated && (b.transform.position - transform.position).sqrMagnitude < ProjectileBurstDistance.Sqr());
      var avgPos = Vector3.zero;
      var numHoles = 0;
      foreach (var hole in activatedHoles) {  // includes self
        avgPos += hole.transform.position;
        numHoles++;
      }
      if (numHoles > 1) {
        avgPos /= numHoles;
        foreach (var hole in activatedHoles) {
          var away = (hole.transform.position - avgPos).normalized;
          hole.GetComponent<Controller>().AddPhysicsVelocity(ProjectileBurstForce * away);
        }
      }
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
        b.Suck(this, Gravity, toHole);
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
      SetGlowColor();
      GetComponent<Mob>().BaseScore += 100;
      if (Character.Health >= ExplodeHealth)
        Explode();
    }
  }

  Color? BaseColor;
  void SetGlowColor() {
    if (BaseColor == null)
      BaseColor = GetComponentInChildren<Segment>().GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
    var color = Color.Lerp(BaseColor.Value, GlowColor, GlowCurve.Evaluate(ExplodePct));
    foreach (var segment in GetComponentsInChildren<Segment>())
      segment.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
  }

  [ContextMenu("Explode")]
  void Explode() {
    // TODO: Particles?
    Instantiate(DeathSpawn, transform.position, Quaternion.identity);
    Destroy(gameObject);
  }

  void Start() {
    SetGlowColor();
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