using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics.Math;
using ShowPhysics.Library.Physics.Shapes;
using System;

namespace ShowPhysics.Library.Physics.Steppables.Commands;

public class DrawCommand
{
    public virtual void Draw()
    {
    }
}

public class DrawPolyCommand : DrawCommand
{
    public PolygonShape Shape { get; set; }
}

public class DrawPolyIndexCommand : DrawPolyCommand
{
    public int VertexIndex { get; set; }
}

public class DrawSelectedEdgeCommand : DrawPolyIndexCommand
{
    public DrawSelectedEdgeCommand(PolygonShape shape, int index)
    {
        Shape = shape;
        VertexIndex = index;
    }

    public override void Draw()
    {
        var v1 = Shape.WorldVertices[VertexIndex];
        var v2 = Shape.WorldVertexAfter(VertexIndex);

        Graphics.Mid.Line()
            .Start(v1)
            .End(v2)
            .Color(Theme.EdgeSelected)
            .ThicknessAbs(Theme.ShapeLineThicknessAbs)
            .Stroke();

        Graphics.DrawVertex(v1);
        Graphics.DrawVertex(v2);
    }
}

public class DrawNormalCommand : DrawPolyIndexCommand
{
    public DrawNormalCommand(PolygonShape shape, int index)
    {
        Shape = shape;
        VertexIndex = index;
    }

    public override void Draw()
    {
        var v1 = Shape.WorldVertices[VertexIndex];
        var edge = Shape.WorldEdgeAt(VertexIndex);
        var normal = edge.RightUnitNormal();

        Graphics.Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();

        Graphics.Mid.Vector().Start(v1).End(v1 + normal * 50).Color(Theme.Normals).Stroke();

        Graphics.Mid.Line().Start(v1).End(v1 + normal * 1500).Color(Theme.Normals).Stroke();
    }
}

public class DrawProjectionOnNormalFromBody : DrawNormalCommand
{
    public DrawProjectionOnNormalFromBody(PolygonShape shape, int index, Body target)
        : base(shape, index)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    public Body Target { get; set; }

    public override void Draw()
    {
        var v1 = Shape.WorldVertices[VertexIndex];
        var edge = Shape.WorldEdgeAt(VertexIndex);
        var normal = edge.RightUnitNormal();
        var toCircle = Target.Position - v1;
        var projection = Vector2.Dot(toCircle, normal);
        var v2 = v1 + normal * projection;

        Graphics.Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();

        Graphics.Mid.Vector().Start(v1).End(v2).Color(Theme.Projection).Stroke();

        Graphics.Text.Scale(0.75f).LengthBetween(v1, v2, false).Text(projection.ToString("0.0"));
    }
}

public class DrawVectorToBodyCommand : DrawPolyIndexCommand
{
    public DrawVectorToBodyCommand(PolygonShape shape, int index, Body target)
    {
        Shape = shape;
        VertexIndex = index;
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    public Body Target { get; set; }

    public override void Draw()
    {
        var v1 = Shape.WorldVertices[VertexIndex];
        var v2 = Target.Position;

        Graphics.Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();
        Graphics.Mid.Vector().Start(v1).End(v2).Color(Theme.Normals).Stroke();
    }
}