using UnityEngine;

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  [RuntimeInitializeOnLoadMethod]
  public static void Boot() {
    Debug.LogWarning("GameManager.Boot sets fixed frame rate.");
    Time.fixedDeltaTime = 1f/60f;
  }

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }
}