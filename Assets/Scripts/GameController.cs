using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Round")]
    public int  targetStraw    = 14;
    public int  hearts         = 3;
    public float roundSeconds  = 60f;
    public bool forceFullMinute = true;

    [Header("References")]
    public ItemSpawner spawner;

    int   strawCount = 0;
    float endAt;
    bool  ended = false;

    void Awake()
    {
        // لو تبين تضمنين دقيقة مهما حصل:
        if (forceFullMinute) roundSeconds = 60f;
    }

    void OnEnable()
    {
        GameEvents.OnStrawCollected += HandleStraw;
        GameEvents.OnHeartLost      += HandleHeartLost;
        GameEvents.OnInstantFail    += HandleInstantFail;
    }

    void OnDisable()
    {
        GameEvents.OnStrawCollected -= HandleStraw;
        GameEvents.OnHeartLost      -= HandleHeartLost;
        GameEvents.OnInstantFail    -= HandleInstantFail;
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
            EndRound(false);
            GameEvents.RaiseTimeUp();
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
        if (!forceFullMinute && strawCount >= targetStraw)
        {
            EndRound(true);
            GameEvents.RaiseGoalReached();
        }
        else if (forceFullMinute && strawCount >= targetStraw)
        {
            GameEvents.RaiseGoalReached();
        }
    }

    void HandleHeartLost(int amt)
    {
        if (ended) return;
        hearts -= Mathf.Abs(amt);
        if (!forceFullMinute && hearts <= 0) EndRound(false);
        if (hearts < 0) hearts = 0;
    }

    void HandleInstantFail()
    {
        if (ended) return;
        if (!forceFullMinute) EndRound(false);
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

