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

  Vector2 LastAim;
  Vector2 LastMousePos;
  void FixedUpdate() {
    var move = Inputs.GamePlay.Move.ReadValue<Vector2>();
    OnMove?.Invoke(move.XZ());
    var aim = Inputs.GamePlay.Aim.ReadValue<Vector2>();
    if (aim != LastAim) {
      LastAim = aim;
      OnAim?.Invoke(aim.XZ());
    } else {
      var mousePos = Inputs.GamePlay.MouseAim.ReadValue<Vector2>();
      if (LastMousePos != mousePos && GameManager.Instance.Players.Count > 0) {
        LastMousePos = mousePos;
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos).XZ();
        var playerPos = GameManager.Instance.Players[0].transform.position;
        var mouseAim = (worldPos - playerPos).XZ().normalized;
        OnAim?.Invoke(mouseAim.XZ());
      }
    }
    if (Inputs.GamePlay.Bomb.WasPerformedThisFrame())
      OnBomb();
    if (Inputs.GamePlay.NextShot.WasPerformedThisFrame())
      OnDebugShot?.Invoke();
  }
}