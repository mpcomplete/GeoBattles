using UnityEngine;

public enum Direction {
  Left,
  Right,
  Forward,
  Back,
  Radial
}

public class GWVectorGridForce : MonoBehaviour {
  public float Scale = 1;
  public float Radius = 1;
  public Direction Direction = Direction.Radial;
  public bool HasColor;
  public Color Color;
  Vector3 SwapXZtoXY(Vector3 v) => new(v.x, v.z, 0);
  void Update() {
    var direction = SwapXZtoXY(Direction switch {
      Direction.Left => -transform.right,
      Direction.Right => transform.right,
      Direction.Forward => transform.forward,
      Direction.Back => -transform.forward,
      _ => Vector3.zero
    });
    if (Direction == Direction.Radial) {
      Bounds.Instance.VectorGrid.AddGridForce(transform.position, Scale, Radius, Color, HasColor);
    } else {
      Bounds.Instance.VectorGrid.AddGridForce(transform.position, Scale*direction, Radius, Color, HasColor);
    }
  }
}