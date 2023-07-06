using UnityEngine;

public class ScoreMessageManager : MonoBehaviour {
  [SerializeField] WorldSpaceMessage ScoreMessagePrefab;
  [SerializeField] WorldSpaceMessage MultiplierMessagePrefab;

  void Awake() {
    ScoreManager.Instance.ScoreChange += ScoreChange;
    ScoreManager.Instance.MultiplierChange += MultiplierChange;
  }

  void OnDestroy() {
    ScoreManager.Instance.ScoreChange -= ScoreChange;
    ScoreManager.Instance.MultiplierChange -= MultiplierChange;
  }

  void ScoreChange(ScoreEvent scoreEvent) {
    var message = scoreEvent.ScoreChange.ToString();
    var position = scoreEvent.Position;
    WorldSpaceMessageManager.Instance.SpawnMessage(ScoreMessagePrefab, message, position, 1);
  }

  void MultiplierChange(MultiplierEvent multiplierEvent) {
    var message = $"MULTIPLIER x{multiplierEvent.Multiplier}";
    var position = multiplierEvent.Position;
    WorldSpaceMessageManager.Instance.SpawnMessage(MultiplierMessagePrefab, message, position, 1);
  }
}