using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ContactHitbox : MonoBehaviour {
  public Team Team;
  float Damage = 1;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      if (hb.TryAttack(Team.ID, Damage))
        Debug.Log($"{name} contact {other.gameObject.name}");
    }
  }
}