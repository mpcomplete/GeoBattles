using System.Collections;
using UnityEngine;

public class PregameFireworks : MonoBehaviour {
  [SerializeField] Timeval SpawnInterval = Timeval.FromMillis(100);
  [SerializeField] ParticleSystem[] ParticleSystems;

  void Awake() {
    GameManager.Instance.PreGame += StartFireworks;
    GameManager.Instance.StartGame += CleanupFireworks;
  }

  void StartFireworks() {
    StopAllCoroutines();
    StartCoroutine(Fireworks());
  }

  void CleanupFireworks() {
    for (var i = transform.childCount-1; i >= 0; i--) {
      Destroy(transform.GetChild(i).gameObject);
    }
    StopAllCoroutines();
  }

  IEnumerator Fireworks() {
    while (true) {
      var prefab = ParticleSystems.Random();
      var position = new Vector3(
        Random.Range(Bounds.Instance.XMin, Bounds.Instance.XMax),
        0,
        Random.Range(Bounds.Instance.ZMin, Bounds.Instance.ZMax));
      var instance = Instantiate(prefab, transform);
      instance.transform.position = position;
      yield return new WaitForSeconds(SpawnInterval.Seconds);
    }
  }
}