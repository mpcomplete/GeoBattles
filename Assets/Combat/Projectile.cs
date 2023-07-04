using UnityEngine;

namespace Archero {
  [RequireComponent(typeof(Rigidbody))]
  public class Projectile : MonoBehaviour {
    public float Damage = 10;
    public float InitialSpeed = 10;
    public int AttackerTeamId;

    static GameObject _Parent;
    static GameObject Parent => _Parent = _Parent ? _Parent : new GameObject("Projectiles");

    public static Projectile Fire(Projectile prefab, Vector3 position, Quaternion rotation, int attackerTeamId) {
      var p = Instantiate(prefab, position, rotation);
      p.AttackerTeamId = attackerTeamId;
      p.transform.SetParent(Parent.transform, true);
      return p;
    }
    void OnTriggerEnter(Collider other) { // MP: This seems to be called for child objects too?
      if (other.gameObject.TryGetComponent(out Hurtbox hb) && hb.TryAttack(AttackerTeamId, Damage)) {
        Destroy(this.gameObject);
      }
    }
    void OnCollisionEnter(Collision collision) {
      Destroy(this.gameObject);
    }
    void Start() {
      GetComponent<Rigidbody>().AddForce(InitialSpeed*transform.forward, ForceMode.Impulse);
    }
  }
}