using UnityEngine;

[ExecuteAlways]
public class IconLayoutGroup : MonoBehaviour {
  void LateUpdate() {
    var container = GetComponent<RectTransform>();
    var containerHeight = container.rect.height;
    var childCount = container.childCount;
    for (var i = 0; i < childCount; i++) {
      var rect = container.GetChild(i).GetComponent<RectTransform>();
      rect.sizeDelta = new(containerHeight, containerHeight);
    }
  }
}