using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ContactHitbox : MonoBehaviour {
  public Character Owner;
  public Team Team;
  float Damage = 1;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      if (hb.TryAttack(Team.ID, Damage, Owner))
        Debug.Log($"{name} contact {other.gameObject.name}");
    }
  }
}