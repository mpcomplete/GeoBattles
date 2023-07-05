using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterState {
  Spawning,
  Alive,
  Dying,
  Dead
}

public abstract class Character : MonoBehaviour {
  [SerializeField] MonoBehaviour[] Abilities;
  [SerializeField] Timeval SpawnDuration = Timeval.FromMillis(500);
  [SerializeField] Timeval DyingDuration = Timeval.FromMillis(500);

  public int Health;
  public int MaxHealth;
  public GameObject LastAttacker;
  public CharacterState State;

  protected UnityAction<Character> GlobalOnSpawn;
  protected UnityAction<Character> GlobalOnAlive;
  protected UnityAction<Character> GlobalOnDying;
  protected UnityAction<Character> GlobalOnDeath;

  protected virtual void OnSpawn() {}
  protected virtual void OnAlive() {}
  protected virtual void OnDying() {}
  protected virtual void OnDeath() {}

  public void Spawn() {
    StopAllCoroutines();
    StartCoroutine(SpawnRoutine());
  }

  protected void OnHurt(float damage) {
    Damage((int)damage);
  }

  public void Damage(int damage) {
    if (State != CharacterState.Alive)
      return;
    Health -= damage;
    if (Health <= 0) {
      StopAllCoroutines();
      StartCoroutine(KillRoutine());
    }
  }

  public void Kill() {
    StopAllCoroutines();
    StartCoroutine(KillRoutine());
  }

  protected virtual IEnumerator SpawnRoutine() {
    State = CharacterState.Spawning;
    OnSpawn();
    GlobalOnSpawn?.Invoke(this);
    yield return new WaitForSeconds(SpawnDuration.Seconds);
    State = CharacterState.Alive;
    Abilities.ForEach(a => a.enabled = true);
    OnAlive();
    GlobalOnAlive?.Invoke(this);
  }

  protected virtual IEnumerator DamageRoutine() {
    yield return null;
  }

  protected virtual IEnumerator KillRoutine() {
    State = CharacterState.Dying;
    Abilities.ForEach(a => a.enabled = false);
    OnDying();
    GlobalOnDying?.Invoke(this);
    yield return new WaitForSeconds(DyingDuration.Seconds);
    State = CharacterState.Dead;
    OnDeath();
    GlobalOnDeath?.Invoke(this);
    Destroy(gameObject);
  }
}