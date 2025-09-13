using ShowPhysics.Library.Managers.Animation;
using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Physics.Steppables;

public partial class Step
{
    public string Text { get; set; }

    public bool? IsColliding { get; set; }

    public bool IsCompleted { get; set; }

    public Action Draw { get; set; }

    public List<Action> Draws { get; set; }

    public Contact Contact { get; set; }

    public void Reset()
    {
        Text = "";
        IsColliding = null;
        IsCompleted = false;
        Draw = null;
        Draws = null;
    }
}

public partial class StepCirclePoly : Step
{
    public int? FacingEdgeIndex { get; set; }
}

public static class Extensions
{
    public static Step AddDraw(this Step step, Action draw)
    {
        if (step.Draws == null) { step.Draws = new List<Action>(); }
        step.Draws.Add(draw);
        return step;
    }

    public static Step AddDrawAnimatedFloat(this Step step, float maxValue, float duration, Action<float> animFunc)
    {
        var anim = Animations.AddFloat(0, maxValue, duration);
        anim.OnRunning = animFunc;
        return step;
    }

    public static Step ClearDrawAnimations(this Step step)
    {
        Animations.Clear();
        return step;
    }

}