using System;
using UnityEngine;

public class BlackHoleTargetSnakeTail : BlackHoleTarget {
  // Initialized by MoveSnake.
  public MoveSnake SnakeHead;
  public int Index;

  public override void OnEaten(BlackHole hole) {
    if (SnakeHead) {
      SnakeHead.OnTailEaten(Index);
      gameObject.SetActive(false);
    } else {
      Destroy(gameObject);
    }
  }

  public void OnHeadDestroyed() {
    SnakeHead = null;
  }

  void Start() => GameManager.Instance.BlackHoleTargets.Add(this);
  void OnDestroy() => GameManager.Instance.BlackHoleTargets.Remove(this);

}