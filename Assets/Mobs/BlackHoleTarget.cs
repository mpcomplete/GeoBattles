using UnityEngine;

public class BlackHoleTarget : MonoBehaviour {
  public Controller Controller;
  public Character Character;
  [SerializeField] float GravityVulnerability = 1f;

  public virtual void OnEaten(BlackHole hole) {
    if (Character is Mob m)
      Destroy(m.gameObject);
  }

  public void Suck(Vector3 accel) => Controller.AddPhysicsAccel(GravityVulnerability * accel);

  void Start() => GameManager.Instance.BlackHoleTargets.Add(this);
  void OnDestroy() => GameManager.Instance.BlackHoleTargets.Remove(this);
}