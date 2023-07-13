using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager : SingletonBehavior<PauseManager> {
  Inputs Inputs;

  public UnityAction OnPause;
  public UnityAction OnUnpause;
  public bool Paused;

  protected override void AwakeSingleton() {
    Debug.LogWarning("Pause from game over seems to actually open the main menu?");
    Inputs = new();
    Inputs.Enable();
    Inputs.GamePlay.Pause.performed += TogglePause;
  }

  void OnDestroy() {
    Inputs.Dispose();
  }

  void TogglePause(InputAction.CallbackContext ctx) {
    if (!GameManager.Instance.IsGameActive)
      return;

    Debug.Log("Pause occured");
    if (Paused) {
      Paused = false;
      Time.timeScale = 1;
      InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
      OnUnpause?.Invoke();
    } else {
      Paused = true;
      Time.timeScale = 0;
      InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
      OnPause?.Invoke();
    }
  }
}