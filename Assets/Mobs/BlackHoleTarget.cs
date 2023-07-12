using UnityEngine;

public class BlackHoleTarget : MonoBehaviour {
  public Controller Controller;
  public Character Character;
  public float GravityVulnerability = 1f;

  public virtual void OnEaten(BlackHole hole) {
    if (Character is Mob m)
      m.Despawn();
  }

  public virtual void Suck(Vector3 accel) => Controller.AddPhysicsAccel(GravityVulnerability * accel);

  void Start() => GameManager.Instance.BlackHoleTargets.Add(this);
  void OnDestroy() => GameManager.Instance.BlackHoleTargets.Remove(this);
}