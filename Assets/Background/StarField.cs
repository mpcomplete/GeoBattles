using System;
using UnityEngine;

[Serializable]
public class StarLayer {
  public float LayerDistance;
  public int StarCount = 256;
}

public class StarField : MonoBehaviour {
  [SerializeField] GameObject StarPrefab;
  [SerializeField] StarLayer[] StarLayers;
  [SerializeField] float BaseX;
  [SerializeField] float BaseZ;

  public void Start() {
    for (var i = 0; i < StarLayers.Length; i++) {
      var layer = StarLayers[i];
      for (var s = 0; s < layer.StarCount; s++) {
        var star = Instantiate(StarPrefab, transform);
        var x = Mathf.Log(layer.LayerDistance) * UnityEngine.Random.Range(-BaseX, BaseX);
        var y = -StarLayers[i].LayerDistance;
        var z = Mathf.Log(layer.LayerDistance) * UnityEngine.Random.Range(-BaseZ, BaseZ);
        star.transform.position = new Vector3(x, y, z);
      }
    }
  }
}