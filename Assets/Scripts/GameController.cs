using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Round")]
    public int targetStraw = 14;      // عدد القشات المطلوبة
    public int hearts = 3;            // عدد القلوب
    public float roundSeconds = 60f;  // الوقت المتبقي
    public bool forceFullMinute = true;

    [Header("References")]
    public ItemSpawner spawner;

    int strawCount = 0;   // عدد القشات التي جمعها اللاعب
    float endAt;          // وقت انتهاء الجولة
    bool ended = false;   // حالة إذا كانت اللعبة انتهت أو لا

    void Awake()
    {
        if (forceFullMinute) roundSeconds = 60f;
    }

    void OnEnable()
    {
        GameEvents.OnStrawCollected += HandleStraw;
        GameEvents.OnHeartLost      += HandleHeartLost;
        GameEvents.OnInstantFail    += HandleInstantFail;
        GameEvents.OnTimeReduced    += HandleTimeReduced;
        GameEvents.OnCoinCollected  += HandleCoinCollected;  // التعامل مع جمع العملة
    }

    void OnDisable()
    {
        GameEvents.OnStrawCollected -= HandleStraw;
        GameEvents.OnHeartLost      -= HandleHeartLost;
        GameEvents.OnInstantFail    -= HandleInstantFail;
        GameEvents.OnTimeReduced    -= HandleTimeReduced;
        GameEvents.OnCoinCollected  -= HandleCoinCollected;
    }

    void Start()
    {
        Time.timeScale = 1f;
        ended = false;
        strawCount = 0;
        endAt = Time.time + roundSeconds;

        if (spawner)
        {
            spawner.enabled = true;
            spawner.StartSpawning(); // أو autoStart
        }
    }

    void Update()
    {
        if (ended) return;

        float remaining = GetRemainingTime();
        GameEvents.RaiseTimerTick(remaining);

        if (remaining <= 0f)
        {
            // إذا انتهى الوقت وكان اللاعب لم يجمع 14 قشة بعد
            if (strawCount < targetStraw)
            {
                EndRound(false);
                GameEvents.RaiseTimeUp(); // Trigger lose panel
            }
            else
            {
                EndRound(true);
                GameEvents.RaiseGoalReached(); // Trigger win panel
            }
        }
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, endAt - Time.time);
    }

    void HandleStraw()
    {
        if (ended) return;
        strawCount++;

        // Check win condition: must have 14 straws AND time remaining
        float remaining = GetRemainingTime();
        if (strawCount >= targetStraw && remaining > 0f)
        {
            EndRound(true);
            GameEvents.RaiseGoalReached(); // Trigger win panel
        }
    }

    void HandleHeartLost(int amt)
    {
        if (ended) return;
        hearts -= Mathf.Abs(amt);
        if (hearts < 0) hearts = 0;

        // If no hearts left, lose immediately
        if (hearts <= 0)
        {
            EndRound(false);
            GameEvents.RaiseTimeUp(); // Trigger lose panel
        }
    }

    void HandleCoinCollected()
    {
        if (ended) return;
        EndRound(false);  // If coin is collected, immediately lose
        GameEvents.RaiseTimeUp(); // Trigger lose panel
    }

    void HandleInstantFail()
    {
        if (ended) return;
        EndRound(false);
    }

    void HandleTimeReduced(float seconds)
    {
        if (ended) return;
        endAt -= seconds;  // Reduce remaining time by subtracting from endAt
        if (endAt < Time.time) endAt = Time.time;

        float remaining = GetRemainingTime();
        GameEvents.RaiseTimerTick(remaining);
    }

    void EndRound(bool win)
    {
        if (ended) return;
        ended = true;

        if (spawner)
        {
            spawner.StopSpawning();
            spawner.enabled = false;
        }

        foreach (var mover in GameObject.FindObjectsOfType<ItemMover>())
            mover.enabled = false;
    }

    public int GetStraw()  => strawCount;
    public int GetHearts() => Mathf.Max(0, hearts);
}


