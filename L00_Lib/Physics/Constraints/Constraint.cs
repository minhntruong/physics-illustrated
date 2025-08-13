using PhysicsIllustrated.Library.Physics.Mathematics;
using Microsoft.Xna.Framework;
using System;

namespace PhysicsIllustrated.Library.Physics.Constraints;

public abstract class Constraint
{
    public Body BodyA { get; set; }

    public Body BodyB { get; set; }

    public Vector2 AnchorA { get; set; } // Anchor point in A's local space

    public Vector2 AnchorB { get; set; } // Anchor point in B's local space

    public virtual void PreSolve(float dt)
    {
        // This method should be overridden in derived classes to implement specific constraint solving logic.
        throw new NotImplementedException("Override this method in derived classes.");
    }

    public virtual void Solve()
    {
        // This method should be overridden in derived classes to implement specific constraint solving logic.
        throw new NotImplementedException("Override this method in derived classes.");
    }

    public virtual void PostSolve()
    {
        // This method can be overridden in derived classes to implement any post-solve logic.
        throw new NotImplementedException("Override this method in derived classes.");
    }

    public VectorN GetVelocities()
    {
        var a = BodyA;
        var b = BodyB;

        var v = new VectorN(6);

        v[0] = a.Velocity.X;
        v[1] = a.Velocity.Y;
        v[2] = a.AngularVelocity;
        v[3] = b.Velocity.X;
        v[4] = b.Velocity.Y;
        v[5] = b.AngularVelocity;

        return v;
    }

    public MatMN GetInvM()
    {
        var a = BodyA;
        var b = BodyB;

        MatMN invM = new MatMN(6, 6);

        invM[0, 0] = a.InvMass;
        invM[1, 1] = a.InvMass;
        invM[2, 2] = a.InvI;
        invM[3, 3] = b.InvMass;
        invM[4, 4] = b.InvMass;
        invM[5, 5] = b.InvI;

        return invM;
    }
}
