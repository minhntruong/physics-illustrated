using System;

namespace ShowPhysics.Library.Managers.Animation;

public class FloatAnimator
{
    public float Start { get; }
    public float End { get; }
    public float Duration { get; }
    public float Elapsed { get; private set; }
    public bool IsCompleted => Elapsed >= Duration;

    public float Current { get; private set; }

    public Action<float> OnRunning { get; set; }

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
        if (IsCompleted) return;
        Elapsed += deltaTime;
        float t = Math.Clamp(Elapsed / Duration, 0f, 1f);
        Current = Lerp(Start, End, t);

        OnRunning?.Invoke(Current);
    }

    public void Reset()
    {
        Elapsed = 0f;
        Current = Start;
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}
