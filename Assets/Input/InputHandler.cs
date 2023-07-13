using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-400)]
public class InputHandler : MonoBehaviour {
  Inputs Inputs;

  public UnityAction<Vector3> OnMove;
  public UnityAction<Vector3> OnAim;
  public UnityAction OnBomb;
  public UnityAction OnDebugShot;

  void Awake() {
    Inputs = new();
    Inputs.Enable();
  }

  void OnDestroy() {
    Inputs.Dispose();
  }

  void FixedUpdate() {
    var move = Inputs.GamePlay.Move.ReadValue<Vector2>();
    OnMove?.Invoke(new Vector3(move.x, 0, move.y));
    var aim = Inputs.GamePlay.Aim.ReadValue<Vector2>();
    OnAim?.Invoke(new Vector3(aim.x, 0, aim.y));
    if (Inputs.GamePlay.Bomb.WasPerformedThisFrame())
      OnBomb();
    if (Inputs.GamePlay.NextShot.WasPerformedThisFrame())
      OnDebugShot?.Invoke();
  }
}