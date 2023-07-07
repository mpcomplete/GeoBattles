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
  public Character LastAttacker;
  public CharacterState State;
  public bool IsAlive => State == CharacterState.Spawning || State == CharacterState.Alive;

  // Global channel (provided by GameManager)
  protected UnityAction<Character> GlobalOnSpawn;
  protected UnityAction<Character> GlobalOnAlive;
  protected UnityAction<Character> GlobalOnDying;
  protected UnityAction<Character> GlobalOnDeath;
  protected UnityAction<Character> GlobalOnDespawn;

  // Local channel for use by self or others interested in us
  public UnityAction OnSpawn;
  public UnityAction OnAlive;
  public UnityAction OnDying;
  public UnityAction OnDeath;
  public UnityAction OnDespawn;

  public void Spawn() {
    StopAllCoroutines();
    StartCoroutine(SpawnRoutine());
  }

  public void Damage(int damage, Character attacker = null) {
    if (!IsAlive)
      return;
    Health -= damage;
    LastAttacker = attacker;
    SendMessage("OnHurt", damage, SendMessageOptions.DontRequireReceiver);
    if (Health <= 0) {
      StopAllCoroutines();
      StartCoroutine(KillRoutine());
    }
  }

  public void Kill() {
    StopAllCoroutines();
    StartCoroutine(KillRoutine());
  }

  public void Despawn() {
    StopAllCoroutines();
    StartCoroutine(DespawnRoutine());
  }

  protected virtual IEnumerator SpawnRoutine() {
    State = CharacterState.Spawning;
    OnSpawn?.Invoke();
    GlobalOnSpawn?.Invoke(this);
    yield return new WaitForSeconds(SpawnDuration.Seconds);
    State = CharacterState.Alive;
    Abilities.ForEach(a => a.enabled = true);
    OnAlive?.Invoke();
    GlobalOnAlive?.Invoke(this);
  }

  protected virtual IEnumerator DamageRoutine() {
    yield return null;
  }

  protected virtual IEnumerator KillRoutine() {
    State = CharacterState.Dying;
    Abilities.ForEach(a => a.enabled = false);
    OnDying?.Invoke();
    GlobalOnDying?.Invoke(this);
    yield return new WaitForSeconds(DyingDuration.Seconds);
    State = CharacterState.Dead;
    OnDeath?.Invoke();
    GlobalOnDeath?.Invoke(this);
    Destroy(gameObject);
  }

  protected virtual IEnumerator DespawnRoutine() {
    State = CharacterState.Dead;
    Abilities.ForEach(a => a.enabled = false);
    OnDespawn?.Invoke();
    GlobalOnDespawn?.Invoke(this);
    yield return new WaitForSeconds(DyingDuration.Seconds);
    Destroy(gameObject);
  }

  void Awake() {
    Health = MaxHealth;
  }
}