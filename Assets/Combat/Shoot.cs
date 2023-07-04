using Archero;
using UnityEngine;

// Identifier for which team a character is on - e.g., player or mob.
public class Shoot : MonoBehaviour {
  public Projectile Projectile;
  public int Count;  // Changes in runtime

  //public struct GunType {
  //  public int Count;
  //  public float HorizontalSpacing;
  //  public float[] Angles;
  //}

  //void FireArrowVolley(Vector3 direction, int count) {
  //  var spreadDirection = Vector3.Cross(direction, Vector3.up);
  //  var spreadTotal = count == 0 ? 0 : (count-1) * SpreadDistance;
  //  var spreadHalf = spreadTotal / 2;
  //  for (var i = 0; i < count; i++) {
  //    var origin = transform.position + ShootOffset + (i * SpreadDistance - spreadHalf) * spreadDirection;
  //    Projectile.Fire(ProjectilePrefab, origin, Quaternion.LookRotation(direction), Attributes, HitConfig);
  //  }
  //}

  //public void Shoot(Vector3 direction) {

  //}
}