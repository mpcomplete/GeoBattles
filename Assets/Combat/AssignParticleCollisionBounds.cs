using UnityEngine;

public class AssignParticleCollisionBounds : MonoBehaviour {
  [SerializeField] ParticleSystem ParticleSystem;

  void Start() {
    ParticleSystem.collision.AddPlane(Bounds.Instance.NorthPlane);
    ParticleSystem.collision.AddPlane(Bounds.Instance.SouthPlane);
    ParticleSystem.collision.AddPlane(Bounds.Instance.WestPlane);
    ParticleSystem.collision.AddPlane(Bounds.Instance.EastPlane);
  }
}
