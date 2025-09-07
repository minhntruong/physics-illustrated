using ShowPhysics.Library.Physics.Steppables.Commands;
using System;

namespace ShowPhysics.Library.Physics.Steppables;

public class Step
{
    public string Name { get; set; }

    public bool? IsColliding { get; set; }

    public bool IsCompleted { get; set; }

    public Action Draw { get; set; }

    public DrawCommand[] Commands { get; set; }
}