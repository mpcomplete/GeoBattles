using UnityEngine;

public class Shield : MonoBehaviour {
  [SerializeField] Character Character;
  [SerializeField] GameObject Model;
  [SerializeField] Timeval Duration = Timeval.FromSeconds(3);
  [SerializeField] Timeval PulseThreshold = Timeval.FromSeconds(1);
  [SerializeField] Timeval PulseOffDuration = Timeval.FromMillis(30);
  [SerializeField] Timeval PulseOnDuration = Timeval.FromMillis(100);

  public bool Raised => TicksRemaining > 0;

  int TicksRemaining;
  int TicksTillPulse;
  bool IsPulseOn;

  void Start() {
    Character.OnSpawn += Raise;
    Character.OnDeath += Lower;
  }

  void OnDestroy() {
    Character.OnSpawn -= Raise;
    Character.OnDeath -= Lower;
  }

  void Raise() {
    TicksRemaining = Duration.Ticks;
    Model.SetActive(true);
  }

  void Lower() {
    TicksRemaining = 0;
    Model.SetActive(false);
  }

  void PulseOn() {
    IsPulseOn = true;
    TicksTillPulse = PulseOnDuration.Ticks;
    Model.SetActive(true);
  }

  void PulseOff() {
    IsPulseOn = false;
    TicksTillPulse = PulseOffDuration.Ticks;
    Model.SetActive(false);
  }

  void FixedUpdate() {
    TicksRemaining--;
    TicksTillPulse--;
    if (TicksRemaining <= 0) {
      Lower();
      Model.SetActive(false);
    } else if (TicksRemaining < PulseThreshold.Ticks && TicksTillPulse <= 0) {
      if (!IsPulseOn)
        PulseOn();
      else
        PulseOff();
    }
  }
}