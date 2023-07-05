using System;
using UnityEngine;

public class SpawnOnDeath : MonoBehaviour {
  public GameObject Spawn;

  void Start() {
    GetComponent<Character>().OnDying += OnDeath;
  }

  void OnDeath() {
    Instantiate(Spawn, transform.position, Quaternion.identity);
  }
}