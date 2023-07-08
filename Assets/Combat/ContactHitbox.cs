using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ContactHitbox : MonoBehaviour {
  public Character Owner;
  public Team Team;
  float Damage = 1;

  void OnEnable() => GetComponent<Collider>().enabled = true;
  void OnDisable() => GetComponent<Collider>().enabled = false;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      if (hb.TryAttack(Team.ID, Damage, Owner))
        Debug.Log($"{name} contact {other.gameObject.name}");
    }
  }

  // So we can be disabled in the editor.
  void FixedUpdate() { }
}