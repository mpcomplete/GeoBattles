using UnityEngine;

public class ContactHitbox : MonoBehaviour {
  public Character Owner;
  public Team Team;
  float Damage = 1;

  void OnEnable() => GetComponent<Collider>().enabled = true;
  void OnDisable() => GetComponent<Collider>().enabled = false;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      hb.TryAttack(Team.ID, Damage, Owner);
    }
  }

  // So we can be disabled in the editor.
  void FixedUpdate() { }
}