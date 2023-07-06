using System.Collections;
using UnityEngine;

public class FaceShield : MonoBehaviour {
  public GameObject Shield;
  public GameObject ShieldModel;
  public int NumFlickers = 5;
  public float MinActivateDelay = .05f;
  public float MaxActivateDelay = .25f;
  public float OnDuration = .2f;
  public float OffDuration = .2f;

  void OnCharge() {
    StartCoroutine(ActivateShield());
  }

  IEnumerator ActivateShield() {
    yield return new WaitForSeconds(Random.Range(MinActivateDelay, MaxActivateDelay));
    Shield.SetActive(true);
    for (int i = 0; i < NumFlickers; i++) {
      ShieldModel.SetActive(true);
      yield return new WaitForSeconds(OnDuration);
      ShieldModel.SetActive(false);
      yield return new WaitForSeconds(OffDuration);
    }
    Shield.SetActive(false);
  }
}