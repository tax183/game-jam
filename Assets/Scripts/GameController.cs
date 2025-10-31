
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Round")]
    public int targetStraw = 14;
    public int hearts = 3;
    public float roundSeconds = 40f;

    [Header("References")]
    public ItemSpawner spawner;

    int strawCount = 0;
    float endAt;
    bool ended = false;

    void OnEnable(){
        GameEvents.OnStrawCollected += HandleStraw;
        GameEvents.OnHeartLost += HandleHeartLost;
        GameEvents.OnInstantFail += HandleInstantFail;
    }
    void OnDisable(){
        GameEvents.OnStrawCollected -= HandleStraw;
        GameEvents.OnHeartLost -= HandleHeartLost;
        GameEvents.OnInstantFail -= HandleInstantFail;
    }

    void Start(){ endAt = Time.time + roundSeconds; ended = false; }

    void Update(){
        if (ended) return;
        float remaining = Mathf.Max(0f, endAt - Time.time);
        GameEvents.RaiseTimerTick(remaining);
        if (remaining <= 0f){ EndRound(false); GameEvents.RaiseTimeUp(); }
    }

    void HandleStraw(){
        if (ended) return;
        strawCount++;
        if (strawCount >= targetStraw){ EndRound(true); GameEvents.RaiseGoalReached(); }
    }
    void HandleHeartLost(int amt){
        if (ended) return;
        hearts -= Mathf.Abs(amt);
        if (hearts <= 0) EndRound(false);
    }
    void HandleInstantFail(){ if (!ended) EndRound(false); }

    void EndRound(bool win){
        ended = true;
        if (spawner) spawner.enabled = false;
        foreach (var mover in GameObject.FindObjectsOfType<ItemMover>()) mover.enabled = false;
    }

    public int GetStraw() => strawCount;
    public int GetHearts() => Mathf.Max(0, hearts);
}
