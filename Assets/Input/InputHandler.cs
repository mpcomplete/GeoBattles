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

  bool MouseFiring = false;
  void FixedUpdate() {
    var move = Inputs.GamePlay.Move.ReadValue<Vector2>();
    OnMove?.Invoke(move.XZ());
    if (MouseFiring && GameManager.Instance.Players.Count > 0) {
      var mousePos = Inputs.GamePlay.MouseAim.ReadValue<Vector2>();
      var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.y)).XZ();
      var playerPos = GameManager.Instance.Players[0].transform.position;
      var mouseAim = (worldPos - playerPos).XZ().normalized;
      OnAim?.Invoke(mouseAim.XZ());
    } else {
      var aim = Inputs.GamePlay.Aim.ReadValue<Vector2>();
      OnAim?.Invoke(aim.XZ());
    }
    if (Inputs.GamePlay.MouseFireToggle.WasPerformedThisFrame())
      MouseFiring = !MouseFiring;
    if (Inputs.GamePlay.Bomb.WasPerformedThisFrame())
      OnBomb();
    if (Inputs.GamePlay.NextShot.WasPerformedThisFrame())
      OnDebugShot?.Invoke();
  }
}