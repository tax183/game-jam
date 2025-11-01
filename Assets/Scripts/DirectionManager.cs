using UnityEngine;

public class DirectionManager : MonoBehaviour
{
    [Header("Switch Settings")]
    public float switchEverySeconds = 60f;
    public bool ignoreTimeUp = true; // ✅ لا تتوقف على TimeUp مؤقتًا

    public int Direction { get; private set; } = 1;

    float nextSwitch;
    bool  active = true;

    void OnEnable()
    {
        if (!ignoreTimeUp) GameEvents.OnTimeUp += HandleTimeUp;
    }

    void OnDisable()
    {
        if (!ignoreTimeUp) GameEvents.OnTimeUp -= HandleTimeUp;
    }

    void Start()
    {
        active = true;
        Direction = +1;
        nextSwitch = Time.time + switchEverySeconds;
        GameEvents.RaiseDirection(Direction);
    }

    void Update()
    {
        if (!active) return;

        if (Time.time >= nextSwitch)
        {
            Direction = -Direction;
            nextSwitch = Time.time + switchEverySeconds;
            GameEvents.RaiseDirection(Direction);
        }
    }

    void HandleTimeUp() { active = false; enabled = false; }
}


