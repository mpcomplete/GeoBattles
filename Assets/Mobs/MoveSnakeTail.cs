using System;
using System.Linq;
using UnityEngine;

public class MoveSnakeTail : MonoBehaviour {
  public Controller SnakeHead;
  public Controller[] TailBones;
  public float TailBoneSeparationDist = .1f;

  public bool TailEaten => TailEatenIndex >= 0;
  int TailEatenIndex = -1;
  BlackHole EatingHole;

  void Start() {
    transform.SetParent(null);  // Detach the tail from the head so it moves separately.
    for (var i = 0; i < TailBones.Length; i++) {
      if (TailBones[i].TryGetComponent(out BlackHoleTargetSnakeTail b))
        b.Index = i;
    }
  }

  void OnDestroy() {
    for (var i = 0; i < TailBones.Length; i++) {
      if (!TailBones[i].TryGetComponent(out BlackHoleTargetSnakeTail b)) {
        // already got eaten
        Destroy(TailBones[i].gameObject);
      }
    }
  }

  void FixedUpdate() {
    if (TailEatenIndex >= 0) {
      SuckToTail();
      return;
    }

    for (int i = TailBones.Length - 1; i >= 0; i--)
      MoveTailBone(TailBones[i], i > 0 ? TailBones[i-1] : SnakeHead, false);
  }

  public void SetMoveSpeed(float maxSpeed) {
    for (var i = 0; i < TailBones.Length; i++) {
      TailBones[i].SetMaxMoveSpeed(maxSpeed);
    }
  }

  void SuckToTail() {
    if (EatingHole == null) {
      if (SnakeHead)
        Destroy(SnakeHead.gameObject);
      Destroy(gameObject);
    }
    if (SnakeHead == null && TailBones.All(tb => !tb.gameObject.activeSelf)) {
      Destroy(gameObject);
      return;
    }
    for (int i = TailBones.Length-1; i > TailEatenIndex; i--)
      MoveTailBone(TailBones[i], i > 0 ? TailBones[i-1] : SnakeHead, false);
    for (int i = TailEatenIndex; i >= 0; i--)
      MoveTailBone(i > 0 ? TailBones[i-1] : SnakeHead, TailBones[i], true);
  }

  void MoveTailBone(Controller tb, Controller tbNext, bool reversed) {
    if (!tb || !tbNext) return; // might have gotten eaten
    var oldPos = tb.transform.position;
    var pos = Vector3.Lerp(tb.transform.position, tbNext.transform.position, Time.fixedDeltaTime / TailBoneSeparationDist);
    var forward = pos - oldPos;
    tb.MoveV(forward / Time.fixedDeltaTime);
    if (forward.sqrMagnitude > .001f)
      tb.Rotation(Quaternion.LookRotation(reversed ? -forward : forward));
  }

  public void OnTailEaten(BlackHole hole, int index) {
    if (TailEatenIndex < 0) {
      EatingHole = hole;
      TailEatenIndex = index;
    }
  }
}