using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Physics.Math;
using M = System.Math;

namespace ShowPhysics.Library.Physics.Constraints;

public class PenetrationConstraint : Constraint
{
    public PenetrationConstraint()
    {
        Jacobian = new MatMN(2, 6);
        CachedLambda = new VectorN(2);
    }

    public PenetrationConstraint(Body a, Body b, Vector2 aCollisionPoint, Vector2 bCollisionPoint, Vector2 normal)
    {
        BodyA = a;
        BodyB = b;

        AnchorA = a.WorldToLocalSpace(aCollisionPoint);
        AnchorB = b.WorldToLocalSpace(bCollisionPoint);

        Normal = a.WorldToLocalSpace(normal);

        Jacobian = new MatMN(2, 6);
        CachedLambda = new VectorN(2);
    }

    private MatMN Jacobian { get; set; }

    private VectorN CachedLambda { get; set; }

    private float Bias { get; set; }

    private Vector2 Normal { get; set; }

    private float Friction { get; set; }

    public override void PreSolve(float dt)
    {
        var a = BodyA;
        var b = BodyB;

        // Get the collision points in world space
        var pa = a.LocalToWorldSpace(AnchorA);
        var pb = b.LocalToWorldSpace(AnchorB);
        var n = a.LocalToWorldSpace(Normal);

        var ra = pa - a.Position;
        var rb = pb - b.Position;

        Jacobian.Zero();

        // Populate the first row of the Jacobian (normal vector)
        var j1 = -n;
        Jacobian[0, 0] = j1.X; // A linear velocity.x
        Jacobian[0, 1] = j1.Y; // A linear velocity.y

        float j2 = -ra.Cross(n);
        Jacobian[0, 2] = j2;   // A angular velocity

        var j3 = n;
        Jacobian[0, 3] = j3.X; // B linear velocity.x
        Jacobian[0, 4] = j3.Y; // B linear velocity.y

        float j4 = rb.Cross(n);
        Jacobian[0, 5] = j4;   // B angular velocity

        // Populate the second row of the Jacobian (friction vector)
        Friction = MathF.Max(a.Friction, b.Friction);

        if (Friction > 0.0f)
        {
            // Perpendicular vector to the normal
            var t = n.RightUnitNormal();

            // First row of the Jacobian (friction vector)
            Jacobian[1, 0] = -t.X; // A linear velocity.x
            Jacobian[1, 1] = -t.Y; // A linear velocity.y
            float j5 = -ra.Cross(t);
            Jacobian[1, 2] = j5;   // A angular velocity
            Jacobian[1, 3] = t.X;  // B linear velocity.x
            Jacobian[1, 4] = t.Y;  // B linear velocity.y
            float j6 = rb.Cross(t);
            Jacobian[1, 5] = j6;   // B angular velocity
        }

        // Warm starting (apply cached lambda)
        MatMN Jt = Jacobian.Transpose();
        VectorN impulses = Jt * CachedLambda;

        // Apply the impulses to both bodies 
        a.ApplyImpulseLinear(new Vector2(impulses[0], impulses[1])); // A linear impulse
        a.ApplyImpulseAngular(impulses[2]);                          // A angular impulse
        b.ApplyImpulseLinear(new Vector2(impulses[3], impulses[4])); // B linear impulse
        b.ApplyImpulseAngular(impulses[5]);                          // B angular impulse

        // Compute the bias term (baumgarte stabilization)
        var beta = 0.2f;
        float c = Vector2.Dot(pb - pa, -n);

        // What is the purpose of this line? Dot product can be negative, so maybe something to do with that
        c = MathF.Min(0.0f, c + 0.01f);

        // Calculate relativie velocity pre-impulse normal, which will be used to compute elasticity
        var va = a.Velocity + new Vector2(-a.AngularVelocity * ra.Y, a.AngularVelocity * ra.X);
        var vb = b.Velocity + new Vector2(-b.AngularVelocity * rb.Y, b.AngularVelocity * rb.X);
        var vrelDotNormal = Vector2.Dot(va - vb, n);

        var e = MathF.Min(a.Restitution, b.Restitution);

        Bias = (beta / dt) * c + (e * vrelDotNormal);
    }

    public override void Solve()
    {
        var a = BodyA;
        var b = BodyB;

        VectorN v = GetVelocities();
        MatMN invM = GetInvM();

        MatMN J = Jacobian;
        MatMN Jt = Jacobian.Transpose();

        // Compute lambda using Ax=b (Gauss-Seidel method) 
        MatMN lhs = J * invM * Jt;  // A
        VectorN rhs = J * v * -1.0f;   // b

        rhs[0] -= Bias;
        VectorN lambda = MatMN.SolveGaussSeidel(lhs, rhs);

        // Accumulate impulses and clamp it within constraint limits
        VectorN oldLambda = CachedLambda;
        CachedLambda += lambda;
        CachedLambda[0] = (CachedLambda[0] < 0.0f) ? 0.0f : CachedLambda[0];

        // Keep friction values between -(λn*µ) and (λn*µ)
        if (Friction > 0.0f)
        {
            var maxFriction = CachedLambda[0] * Friction;            
            CachedLambda[1] = M.Clamp(CachedLambda[1], -maxFriction, maxFriction);
        }

        lambda = CachedLambda - oldLambda;

        // Compute the impulses with both direction and magnitude
        VectorN impulses = Jt * lambda;

        // Apply the impulses to both bodies 
        a.ApplyImpulseLinear(new Vector2(impulses[0], impulses[1])); // A linear impulse
        a.ApplyImpulseAngular(impulses[2]);                          // A angular impulse
        b.ApplyImpulseLinear(new Vector2(impulses[3], impulses[4])); // B linear impulse
        b.ApplyImpulseAngular(impulses[5]);                          // B angular impulse
    }

    public override void PostSolve()
    {
    }
}
