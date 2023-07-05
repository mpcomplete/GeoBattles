using UnityEngine;

public class Hurtbox : MonoBehaviour {
  public GameObject Owner = null;
  public Team Team;

  void Awake() {
    Owner = Owner ?? transform.parent.gameObject;
    Team = Team ?? Owner.GetComponent<Team>();
  }

  public virtual bool CanBeHurtBy(int attackerTeam) {
    if (!Team.CanBeHurtBy(attackerTeam))
      return false;
    return true;
  }

  public virtual bool TryAttack(int attackerTeam, float damage) {
    if (!CanBeHurtBy(attackerTeam)) return false;
    Owner.SendMessage("OnHurt", damage, SendMessageOptions.DontRequireReceiver);
    Debug.Log($"On Try Attack {damage}");
    return true;
  }
}