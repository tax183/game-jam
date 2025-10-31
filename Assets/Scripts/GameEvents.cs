
using System;
using UnityEngine;

public static class GameEvents
{
    public static Action OnStrawCollected;
    public static Action<int> OnHeartLost;
    public static Action OnInstantFail;
    public static Action OnGoalReached;

    public static Action<float> OnTimerTick;
    public static Action OnTimeUp;

    public static Action<float, float> OnInflateHazards; // duration, scale
    public static Action<int> OnDirectionChanged;        // +1 or -1

    public static void RaiseStraw() => OnStrawCollected?.Invoke();
    public static void RaiseHeartLost(int amt) => OnHeartLost?.Invoke(amt);
    public static void RaiseInstantFail() => OnInstantFail?.Invoke();
    public static void RaiseGoalReached() => OnGoalReached?.Invoke();

    public static void RaiseTimerTick(float t) => OnTimerTick?.Invoke(t);
    public static void RaiseTimeUp() => OnTimeUp?.Invoke();

    public static void RaiseInflate(float dur, float scale) => OnInflateHazards?.Invoke(dur, scale);
    public static void RaiseDirection(int dir) => OnDirectionChanged?.Invoke(dir);
}
