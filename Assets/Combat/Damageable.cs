using System;
using UnityEngine;
using UnityEngine.Events;

namespace Archero {
  [Serializable]
  public struct DamageEvent {
    public int Delta;
    public int Health;
    public int MaxHealth;
    public DamageEvent(int delta, int health, int maxHealth) {
      Delta = delta;
      Health = health;
      MaxHealth = maxHealth;
    }
  }

  public class Damageable : MonoBehaviour {
    [SerializeField] public int Health;
    [SerializeField] public int MaxHealth;
    [SerializeField] UnityEvent<DamageEvent> OnDamage;
    [SerializeField] UnityEvent OnDeath;
    GameObject LastAttacker;

    public float HealthPct => Health / (float)MaxHealth;  // Range [0,1]

    void Start() {
      Health = MaxHealth;
    }

    void OnHurt(float damage) {
      TakeDamage((int)damage);
    }

    public void TakeDamage(int damage) {
      if (damage == 0) return;
      if (Health == 0) return; // we're already dead
      Health = Mathf.Max(0, Health - damage);

      var damageEvent = new DamageEvent(-damage, Health, MaxHealth);
      OnDamage.Invoke(damageEvent);
      BroadcastMessage("OnDamage", damageEvent, SendMessageOptions.DontRequireReceiver);

      if (Health <= 0) {
        OnDeath.Invoke();
        BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
      }
    }

    [ContextMenu("Kill")]
    void Kill() {
      TakeDamage(MaxHealth);
    }
  }
}