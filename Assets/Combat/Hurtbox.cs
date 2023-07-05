using UnityEngine;

public class Hurtbox : MonoBehaviour {
  public Character Owner;
  public Team Team;

  void Start() {
    Owner.OnSpawn += OnSpawn;
    Owner.OnDying += OnDying;
  }

  void OnDestroy() {
    Owner.OnSpawn -= OnSpawn;
    Owner.OnDying -= OnDying;
  }

  void OnSpawn() => enabled = true;
  void OnDying() => enabled = false;

  public virtual bool CanBeHurtBy(int attackerTeam) => Team.CanBeHurtBy(attackerTeam);

  public virtual bool TryAttack(int attackerTeam, float damage) {
    if (enabled && CanBeHurtBy(attackerTeam) && Owner.IsAlive) {
      Owner.Damage((int)damage);
      return true;
    } else {
      return false;
    }
  }
}