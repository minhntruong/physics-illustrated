using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Physics.Steppables;

public class Step
{
    public string Name { get; set; }

    public bool? IsColliding { get; set; }

    public bool IsCompleted { get; set; }

    public Action Draw { get; set; }

    public List<Action> Draws { get; set; }

    public void Reset()
    {
        Name = "";
        IsColliding = null;
        IsCompleted = false;
        Draw = null;
        Draws = null;
    }
}

public class StepCirclePoly : Step
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
}