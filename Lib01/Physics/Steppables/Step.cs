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

    public List<Action> Draws { get; } = new List<Action>();

    public Contact Contact { get; set; }

    public void Reset()
    {
        Text = "";
        IsColliding = null;
        IsCompleted = false;
        Draw = null;
        Draws.Clear();
    }
}

public partial class StepCirclePoly : Step
{
    public int? FacingEdgeIndex { get; set; }
}

public static class Extensions
{
    public static Action AddDraw(this Step step, Action draw)
    {
        step.Draws.Add(draw);
        return draw;
    }

    public static Action AddAnim(this Step step, float duration, Action<float> animFunc)
    {
        var anim = Animations.AddFloat(0, 1, duration);

        var drawAction = () => animFunc(anim.Current);

        step.AddDraw(drawAction);

        return drawAction;
    }

    public static Action AddAnim(this Step step, float maxValue, float duration, Action<float> animFunc)
    {
        var anim = Animations.AddFloat(0, maxValue, duration);

        var drawAction = () => animFunc(anim.Current);

        step.AddDraw(drawAction);

        return drawAction;
    }

    public static void RemoveLastDraw(this Step step)
    {
        if (step.Draws.Count > 0)
            step.Draws.RemoveAt(step.Draws.Count - 1);
    }

    public static void RemoveDraw(this Step step, Action draw)
    {
        step.Draws.Remove(draw);
    }

    //public static void ClearDrawAnimations(this Step step)
    //{
    //    Animations.Clear();
    //}
}