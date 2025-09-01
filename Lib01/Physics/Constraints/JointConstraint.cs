using System;
using ShowPhysics.Library.Physics;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Physics.Math;

namespace ShowPhysics.Library.Physics.Constraints;

public class JointConstraint : Constraint
{
    public JointConstraint()
    {
        Jacobian = new MatMN(1, 6);
        CachedLambda = new VectorN(1);
    }

    public JointConstraint(Body a, Body b, Vector2 anchorPoint)
    {
        BodyA = a;
        BodyB = b;

        AnchorA = a.WorldToLocalSpace(anchorPoint);
        AnchorB = b.WorldToLocalSpace(anchorPoint);

        Jacobian = new MatMN(1, 6);
        CachedLambda = new VectorN(1);
    }

    private MatMN Jacobian { get; set; }

    private VectorN CachedLambda { get; set; }

    private float Bias { get; set; }

    public override void PreSolve(float dt)
    {
        var a = BodyA;
        var b = BodyB;

        var pa = a.LocalToWorldSpace(AnchorA);
        var pb = b.LocalToWorldSpace(AnchorB);

        var ra = pa - a.Position;
        var rb = pb - b.Position;

        Jacobian.Zero();

        Vector2 j1 = (pa - pb) * 2.0f;
        Jacobian[0, 0] = j1.X; // A linear velocity.x
        Jacobian[0, 1] = j1.Y; // A linear velocity.y

        float j2 = ra.Cross(pa - pb) * 2.0f;
        Jacobian[0, 2] = j2;   // A angular velocity

        Vector2 j3 = (pb - pa) * 2.0f;
        Jacobian[0, 3] = j3.X; // B linear velocity.x
        Jacobian[0, 4] = j3.Y; // B linear velocity.y

        float j4 = rb.Cross(pb - pa) * 2.0f;
        Jacobian[0, 5] = j4;   // B angular velocity

        // Warm startint (apply cached lambda)

        MatMN Jt = Jacobian.Transpose();

        VectorN impulses = Jt * CachedLambda;

        // Apply the impulses to both A and B
        a.ApplyImpulseLinear(new Vector2(impulses[0], impulses[1])); // A linear impulse
        a.ApplyImpulseAngular(impulses[2]);                          // A angular impulse
        b.ApplyImpulseLinear(new Vector2(impulses[3], impulses[4])); // B linear impulse
        b.ApplyImpulseAngular(impulses[5]);                          // B angular impulse

        // Compute the bias term (baumgarte stabilization)
        const float beta = 0.1f;

        // Compute the positional error
        // The next 2 lines are equivalent:
        //var c = Vector2.Dot(pb - pa, pb - pa);
        var c = (pb - pa).LengthSquared();

        // This line is more accurate, but expensive
        //var c = (pb - pa).Length();

        // What is the purpose of this line?
        c = MathF.Max(0, c - 0.01f);

        Bias = (beta / dt) * c;
    }

    public override void Solve()
    {
        var a = BodyA;
        var b = BodyB;

        VectorN v = GetVelocities();
        MatMN invM = GetInvM();

        MatMN J = Jacobian;
        MatMN Jt = Jacobian.Transpose();

        // Calculate the numerator
        MatMN lhs = J * invM * Jt; // A
        VectorN rhs = J * v * -1.0f;  // b

        rhs[0] -= Bias;

        // Solve the values of lambda using Ax=b (Gaus-Seidel method)
        VectorN lambda = MatMN.SolveGaussSeidel(lhs, rhs);

        CachedLambda += lambda;

        // Compute the final impulses with direction and magnitude
        VectorN impulses = Jt * lambda;

        // Apply the impulses to both A and B
        a.ApplyImpulseLinear(new Vector2(impulses[0], impulses[1])); // A linear impulse
        a.ApplyImpulseAngular(impulses[2]);                          // A angular impulse
        b.ApplyImpulseLinear(new Vector2(impulses[3], impulses[4])); // B linear impulse
        b.ApplyImpulseAngular(impulses[5]);                          // B angular impulse
    }

    public override void PostSolve()
    {
    }

    //==========================================================================

    
}

