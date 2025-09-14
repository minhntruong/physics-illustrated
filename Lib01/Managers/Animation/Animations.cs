using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Managers.Animation;

public static class Animations
{
    private static List<FloatAnimator> _animators = new List<FloatAnimator>();

    public static void Clear()
    {
        _animators.Clear();
    }

    public static void Add(FloatAnimator animator)
    {
        if (animator == null) throw new ArgumentNullException(nameof(animator));
        _animators.Add(animator);
    }

    public static FloatAnimator AddFloat(float start, float end, float duration)
    {
        var animator = new FloatAnimator(start, end, duration);
        _animators.Add(animator);
        return animator;
    }

    public static void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = _animators.Count - 1; i >= 0; i--)
        {
            var animator = _animators[i];

            if (animator.IsCompleted == false)
            {
                animator.Update(deltaTime);
            }
        }
    }
}