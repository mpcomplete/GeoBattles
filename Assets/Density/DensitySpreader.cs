using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionStrategy {
  Alignment,
  Histogram
}

public enum DensityScalingStrategy {
  None,
  Multiply,
  Log
}

public class DensitySpreader : MonoBehaviour {
  public Timeval SpawnInterval = Timeval.FromMillis(100);
  public Transform[] Prefabs;
  [Range(0,2048)]
  public int NumberOfSpawns = 256;
  [Range(0,1)]
  public float Strength = 1.0f;
  [Range(0,1)]
  public float Dampening = 0.95f;
  [Range(0,10)]
  public float MaxSpeed = 5f;
  [Range(0,1)]
  public float RandomJitterScale = .1f;
  public DirectionStrategy DirectionStrategy;
  public DensityScalingStrategy DensityScalingStrategy;

  Vector2[] directions = {
    new Vector3(-1, 0, 0),
    new Vector3(1, 0, 0),
    new Vector3(0, 0, -1),
    new Vector3(0, 0, 1),
    new Vector3(-1, 0, -1),
    new Vector3(-1, 0, 1),
    new Vector3(1, 0, -1),
    new Vector3(1, 0, 1)
  };

  List<Transform> objects = new();
  Dictionary<Transform, Vector3> velocities = new Dictionary<Transform, Vector3>();
  int[,] densityGrid = new int[32, 24];
  Vector3 targetPoint;
  float targetPointSpeed = 1.0f;

  void Start() {
    StartCoroutine(StaggerSpawns());
  }

  IEnumerator StaggerSpawns() {
    var index = 0;
    for (var i = 0; i < NumberOfSpawns; i++) {
      var prefab = Prefabs[index];
      var instance = Instantiate(prefab, transform);
      objects.Add(instance);
      velocities.Add(instance, Vector3.zero);
      index++;
      index = index < Prefabs.Length ? index : 0;
      yield return new WaitForSeconds(SpawnInterval.Seconds);
    }
  }

  void MoveTargetPoint() {
    targetPoint.x = 16 * Mathf.Sin(Time.time * targetPointSpeed);
    targetPoint.z = 12 * Mathf.Cos(Time.time * targetPointSpeed);
  }

  void PopulateDensities() {
    for (int i = 0; i < 32; i++) {
      for (int j = 0; j < 24; j++) {
        densityGrid[i,j] = 0;
      }
    }
    foreach (Transform obj in objects) {
      int x = Mathf.Clamp(Mathf.FloorToInt(obj.position.x + 16), 0, 31);
      int z = Mathf.Clamp(Mathf.FloorToInt(obj.position.z + 12), 0, 23);
      densityGrid[x, z]++;
    }
  }

  // possibly dubious. seems to degenerate and sort of hug the walls
  Vector3? BestDirectionByAlignment(Transform obj) {
    var x = Mathf.Clamp(Mathf.FloorToInt(obj.position.x + 16), 0, 31);
    var z = Mathf.Clamp(Mathf.FloorToInt(obj.position.z + 12), 0, 23);
    var v = velocities[obj];
    var left = new Vector3(-v.z, 0, v.x).normalized;
    var right = -left;
    var currentDensity = densityGrid[x, z];
    Vector3? bestDirection = null;
    var bestAlignment = -Mathf.Infinity;
    for (int i = 0; i < directions.Length; i++) {
      var nx = x + (int)directions[i].x;
      var nz = z + (int)directions[i].y;
      if (nx >= 0 && nx < 32 && nz >= 0 && nz < 24) {
        var neighborDensity = densityGrid[nx, nz];
        if (neighborDensity < currentDensity) {
          var direction = directions[i];
          var alignment1 = Vector3.Dot(left, direction);
          var alignment2 = Vector3.Dot(right, direction);
          var alignment = Mathf.Max(alignment1, alignment2);
          if (alignment > bestAlignment) {
            bestAlignment = alignment;
            bestDirection = direction;
          }
        }
      }
    }
    return bestDirection;
  }

  List<Vector3> weightedDirections = new(256);
  Vector3? BestDirectionByHistogram(Transform obj) {
    var x = Mathf.Clamp(Mathf.FloorToInt(obj.position.x + 16), 0, 31);
    var z = Mathf.Clamp(Mathf.FloorToInt(obj.position.z + 12), 0, 23);
    var currentDensity = densityGrid[x, z];
    var right = new Vector3(-velocities[obj].z, velocities[obj].y, velocities[obj].x);
    var left = right;
    weightedDirections.Clear();
    for (int i = 0; i < directions.Length; i++) {
      var nx = x + (int)directions[i].x;
      var nz = z + (int)directions[i].y;
      if (nx >= 0 && nx < 32 && nz >= 0 && nz < 24) {
        var neighborDensity = densityGrid[nx, nz];
        if (neighborDensity < currentDensity) {
          var direction = directions[i];
          var alongRight = Vector3.Dot(right, direction);
          var alongLeft = Vector3.Dot(left, direction);
          // score our preference for choosing this option be weighing it by a multiplication
          // of the difference in density between the neighbor and ourselves
          // and the strength of alignment with either the left or right vector of our current velocity
          var weight = (int)((currentDensity - neighborDensity) * Mathf.Max(alongRight, alongLeft));
          // add "weight" copies of the vector to our "histogram" to affect the probabilty of sampling them
          for (int j = 0; j < weight; j++)
            weightedDirections.Add(direction);
        }
      }
    }
    return weightedDirections.Count > 0 ? weightedDirections.Random() : null;
  }

  void FixedUpdate() {
    MoveTargetPoint();
    PopulateDensities();
    foreach (Transform obj in objects) {
      var x = Mathf.Clamp(Mathf.FloorToInt(obj.position.x + 16), 0, 31);
      var z = Mathf.Clamp(Mathf.FloorToInt(obj.position.z + 12), 0, 23);
      var currentDensity = densityGrid[x, z];
      var bestDirection = DirectionStrategy switch {
        DirectionStrategy.Alignment => BestDirectionByAlignment(obj),
        DirectionStrategy.Histogram => BestDirectionByHistogram(obj)
      };
      velocities[obj] *= Dampening;
      if (bestDirection.HasValue) {
        var densityScale = DensityScalingStrategy switch {
          DensityScalingStrategy.None => 1,
          DensityScalingStrategy.Multiply => currentDensity,
          DensityScalingStrategy.Log => Mathf.Log(currentDensity),
        };
        var orthogonalForce = bestDirection.Value - Vector3.Project(bestDirection.Value, velocities[obj]);
        var force = orthogonalForce * Strength * densityScale;
        velocities[obj] += force;
      }
      velocities[obj] += (targetPoint - obj.position).normalized;
      velocities[obj] += RandomJitterScale * Random.insideUnitSphere.XZ();
      velocities[obj] = Mathf.Min(velocities[obj].magnitude, MaxSpeed) * velocities[obj].normalized;
      obj.position += velocities[obj] * Time.deltaTime;
    }
  }
}
