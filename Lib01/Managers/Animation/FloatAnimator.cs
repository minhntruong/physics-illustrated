using System;
using static ShowPhysics.Library.Managers.Animation.Transitions;

namespace ShowPhysics.Library.Managers.Animation;

public class FloatAnimator
{
    public float Start { get; }
    public float End { get; }
    public float Duration { get; }
    public float Elapsed { get; private set; }
    public bool IsCompleted => Elapsed >= Duration;
    public float Current { get; private set; }

    // Easing function delegate
    public Func<float, float> Easing { get; set; } = EasingFunctions.EaseInOutQuad;

    public FloatAnimator(float start, float end, float duration)
    {
        Start = start;
        End = end;
        Duration = duration;
        Elapsed = 0f;
        Current = start;
    }

    public void Update(float deltaTime)
    {
        if (IsCompleted) { return; }

        Elapsed += deltaTime;
        float t = Math.Clamp(Elapsed / Duration, 0f, 1f);
        t = Easing(t); // Apply easing
        Current = Lerp(Start, End, t);
    }

    public void Reset()
    {
        Elapsed = 0f;
        Current = Start;
    }

    //private static float Lerp(float a, float b, float t)
    //{
    //    return a + (b - a) * t;
    //}
}

// Common easing functions
public static class EasingFunctions
{
    public static float Linear(float t) => t;
    public static float EaseInQuad(float t) => t * t;
    public static float EaseOutQuad(float t) => t * (2 - t);
    public static float EaseInOutQuad(float t) =>
        t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
}
