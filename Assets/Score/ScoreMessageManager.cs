using UnityEngine;

public class ScoreMessageManager : MonoBehaviour {
  [SerializeField] WorldSpaceMessage ScoreMessagePrefab;
  [SerializeField] WorldSpaceMessage MultiplierMessagePrefab;

  void Start() {
    ScoreManager.Instance.ScoreChange += ScoreChange;
    ScoreManager.Instance.MultiplierChange += MultiplierChange;
  }

  void OnDestroy() {
    ScoreManager.Instance.ScoreChange -= ScoreChange;
    ScoreManager.Instance.MultiplierChange -= MultiplierChange;
  }

  void ScoreChange(ScoreEvent scoreEvent) {
    var score = (scoreEvent.Character as Mob).Score * scoreEvent.Multiplier;
    var message = score.ToString();
    var position = scoreEvent.Character.transform.position;
    WorldSpaceMessageManager.Instance.SpawnMessage(ScoreMessagePrefab, message, position, 1);
  }

  void MultiplierChange(ScoreEvent scoreEvent) {
    var message = $"MULTIPLIER x{scoreEvent.Multiplier}";
    var position = scoreEvent.Character.transform.position;
    WorldSpaceMessageManager.Instance.SpawnMessage(MultiplierMessagePrefab, message, position, 1);
  }
}