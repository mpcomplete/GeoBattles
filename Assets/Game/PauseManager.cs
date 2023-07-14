using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager : SingletonBehavior<PauseManager> {
  Inputs Inputs;

  public UnityAction OnPause;
  public UnityAction OnUnpause;
  public bool Paused;

  protected override void AwakeSingleton() {
    Inputs = new();
    Inputs.Enable();
    Inputs.GamePlay.Pause.performed += TogglePause;
  }

  void TogglePause(InputAction.CallbackContext ctx) {
    switch (GameManager.Instance.GameState) {
      case GameState.PreGame:
        GameManager.Instance.LoadLevel();
      break;

      case GameState.PostGame:
        GameManager.Instance.LoadMainMenu();
      break;

      case GameState.InGame:
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
      break;
    }
  }
}