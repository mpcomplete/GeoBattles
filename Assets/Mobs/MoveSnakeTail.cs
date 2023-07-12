using System.Linq;
using UnityEngine;

public class MoveSnakeTail : MonoBehaviour {
  public Transform SnakeHead;
  public Transform[] TailBones;
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
      MoveTailBone(TailBones[i].transform, i > 0 ? TailBones[i-1].transform : SnakeHead, false);
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
      MoveTailBone(TailBones[i].transform, i > 0 ? TailBones[i-1].transform : SnakeHead, false);
    for (int i = TailEatenIndex; i >= 0; i--)
      MoveTailBone(i > 0 ? TailBones[i-1].transform : SnakeHead, TailBones[i].transform, true);
  }

  void MoveTailBone(Transform tb, Transform tbNext, bool reversed) {
    if (!tb || !tbNext) return; // might have gotten eaten
    var oldPos = tb.position;
    tb.position = Vector3.Lerp(tb.position, tbNext.position, Time.fixedDeltaTime / TailBoneSeparationDist);
    var forward = tb.position - oldPos;
    if (forward.sqrMagnitude > .001f)
      tb.forward = reversed ? -forward : forward;
  }

  public void OnTailEaten(BlackHole hole, int index) {
    if (TailEatenIndex < 0) {
      EatingHole = hole;
      TailEatenIndex = index;
    }
  }
}