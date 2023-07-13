using UnityEngine;

public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour {
  public static T Instance;

  protected virtual void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this as T;
      DontDestroyOnLoad(gameObject);
      AwakeSingleton();
    }
  }

  protected virtual void AwakeSingleton() { }
}