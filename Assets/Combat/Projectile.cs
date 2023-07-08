using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
  static GameObject _Parent;
  static GameObject Parent => _Parent = _Parent ? _Parent : new GameObject("Projectiles");

  public static Projectile Fire(Projectile prefab, Vector3 position, Quaternion rotation, int attackerTeamId) {
    var p = Instantiate(prefab, position, rotation);
    p.AttackerTeamId = attackerTeamId;
    p.transform.SetParent(Parent.transform, true);
    return p;
  }

  public float Damage = 10;
  public float InitialSpeed = 10;
  public int AttackerTeamId;
  public ParticleSystem ParticleSystemPrefab;
  public Rigidbody Rigidbody;

  void Start() {
    this.InitComponent(out Rigidbody);
    Rigidbody.AddForce(InitialSpeed*transform.forward, ForceMode.VelocityChange);
    GameManager.Instance.ProjectileSpawn.Invoke(this);
  }

  void OnDestroy() {
    GameManager.Instance.ProjectileDeath.Invoke(this);
  }

  void OnTriggerEnter(Collider other) { // MP: This seems to be called for child objects too?
    if (other.gameObject.TryGetComponent(out Hurtbox hb) && hb.TryAttack(AttackerTeamId, Damage, null)) {
      Destroy(gameObject);
    }
  }

  void OnCollisionEnter(Collision collision) {
    var contact = collision.contacts[0];
    var position = contact.point;
    var rotation = Quaternion.LookRotation(-contact.normal); // -normal because particle system is setup this way
    Instantiate(ParticleSystemPrefab, position, rotation);
    Destroy(gameObject);
  }
}