using System.Collections;
using UnityEngine;

public class ShatterTail : MonoBehaviour {
  [SerializeField] Character Character;
  [SerializeField] Transform[] TailBones;
  [SerializeField] float Delay = 0.1f;
  [SerializeField] float ExplosionForce = 5f;

  void Start() {
    Character.OnDying += Shatter;
    Character.OnDespawn += Shatter;
  }

  void OnDestroy() {
    Character.OnDying -= Shatter;
    Character.OnDespawn -= Shatter;
  }

  void Shatter() {
    StartCoroutine(ShatterRoutine());
  }

  IEnumerator ShatterRoutine() {
    // Disable colliders and hitboxes first.
    for (int i = 0; i < TailBones.Length; i++)
      TailBones[i].GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false);
    for (int i = 0; i < TailBones.Length; i++) {
      TailBones[i].GetComponentsInChildren<Segment>().ForEach(s => s.Break(ExplosionForce, TailBones[i].transform.position));
      //foreach (var segment in TailBones[i].GetComponentsInChildren<Segment>())
      //    segment.Break(ExplosionForce, TailBones[i].transform.position);
      yield return new WaitForSeconds(Delay);
    }
    Destroy(gameObject);
  }
}