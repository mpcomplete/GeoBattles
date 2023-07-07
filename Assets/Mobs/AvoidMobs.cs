using UnityEngine;

public class AvoidMobs : MonoBehaviour {
  [SerializeField] Controller Controller;
  //[SerializeField] float Acceleration = 2;
  //[SerializeField] float MaxSpeed = 2;
  [SerializeField] float SeparationDistance = 2f;
  [SerializeField] float SeparationStrength = 1f;

  void FixedUpdate() {
    var others = GameManager.Instance.Mobs;
    var nearAvg = Vector3.zero;
    foreach (var mob in others) {
      var delta = transform.position - mob.transform.position;
      if (delta.sqrMagnitude < SeparationDistance.Sqr())
        nearAvg += delta;
    }
    var velocity = SeparationStrength * nearAvg;
    //if (Velocity.sqrMagnitude > MaxSpeed.Sqr())
    //  Velocity = MaxSpeed * Velocity.normalized;
    Controller.Move(Time.fixedDeltaTime * velocity);
  }
}