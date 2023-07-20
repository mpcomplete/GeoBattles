using System;
using UnityEngine;

public class ObjectTracker : MonoBehaviour {
  public Action OnDestroyed;

  void OnDestroy() {
    OnDestroyed?.Invoke();
  }
}