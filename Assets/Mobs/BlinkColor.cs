using System.Collections;
using UnityEngine;

public class BlinkColor : MonoBehaviour {
  const string COLOR_NAME = "_EmissionColor";

  [SerializeField] Character Character;
  [SerializeField] MeshRenderer MeshRenderer;
  [SerializeField] AnimationCurve BlinkCurve;
  [SerializeField] Timeval BlinkDuration = Timeval.FromSeconds(1);
  [SerializeField, ColorUsage(hdr:true, showAlpha:true)] Color Color;
  Color BaseColor;

  void Start() {
    Character.OnAlive += StartBlinking;
    Character.OnDamage += StopBlinking;
    BaseColor = MeshRenderer.material.GetColor(COLOR_NAME);
  }

  void OnDestroy() {
    Character.OnAlive -= StartBlinking;
    Character.OnDamage -= StopBlinking;
  }

  IEnumerator Blink() {
    while (true) {
      var ticks = (float)BlinkDuration.Ticks;
      var t = (float)Timeval.TickCount % (float)BlinkDuration.Ticks;
      var i = BlinkCurve.Evaluate(t/ticks);
      var c = Color.Lerp(BaseColor, Color, i);
      MeshRenderer.material.SetColor(COLOR_NAME, c);
      yield return new WaitForFixedUpdate();
    }
  }

  void StartBlinking() {
    StartCoroutine(Blink());
  }

  void StopBlinking() {
    Character.OnDamage -= StopBlinking;
    Character.OnAlive -= StartBlinking;
    MeshRenderer.material.SetColor(COLOR_NAME, BaseColor);
    StopAllCoroutines();
  }
}