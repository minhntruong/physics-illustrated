using ShowPhysics.Library.Physics.Steppables.Commands;
using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Physics.Steppables;

public class Step
{
    public string Name { get; set; }

    public bool? IsColliding { get; set; }

    public bool IsCompleted { get; set; }

    public Action Draw { get; set; }

    public List<DrawCommand> Commands { get; set; }
}

public static class Extensions
{
    public static Step AddCommand(this Step step, DrawCommand command)
    {
        if (step.Commands == null) { step.Commands = new List<DrawCommand>(); }

        step.Commands.Add(command);
        return step;
    }
}