using System;
using ShowPhysics.Library.Physics.Shapes;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Physics.Math;

namespace ShowPhysics.Library.Physics;

public interface IId
{
    int Id { get; set; }
}

public class Body : IId
{
    public Body(Shape shape, float x, float y, float mass)
    {
        CommonInit(shape, new Vector2(x, y), mass);
    }

    public Body(Shape shape, Vector2 position, float mass)
    {
        CommonInit(shape, position, mass);
    }

    private void CommonInit(Shape shape, Vector2 position, float mass)
    {
        Shape = shape;

        Position = position;

        Mass = mass;
        InvMass = Mass > 0 ? 1 / Mass : 0;

        I = shape.MomentOfInertiaFactor * InvMass;
        InvI = I > 0 ? 1 / I : 0;

        Shape.UpdateVertices(Rotation, Position);
    }

    public override string ToString()
    {
        return $"{Id} \"{Name}\" {Shape.Type} {Position}";
    }

    public int Id { get; set; } = -1; // Unique identifier for the body, useful for debugging and collision detection

    public string Name { get; set; } = "";

    public Shape Shape { get; set; }

    //=== Linear motion ========================================================
    
    public Vector2 Position { get; set; }
    
    public Vector2 Velocity { get; set; }
    
    public Vector2 Acceleration { get; set; }

    //=== Angular motion =======================================================
    
    public float Rotation { get; set; }
    
    public float AngularVelocity { get; set; }
    
    public float AngularAcceleration { get; set; }

    //=== Force and torque =====================================================
    
    public Vector2 SumForces { get; set; }
    
    public float SumTorque { get; set; }

    //=== Mass and Moment of Inertia ===========================================
    public float Mass { get; set; }

    public float InvMass { get; set; }
    
    public float I {  get; set; }
    
    public float InvI { get; set; }

    public bool IsStatic => InvMass == 0;

    public float Restitution { get; set; } = 1.0f; // Coefficient of restitution, 1.0 means perfectly elastic collision

    public float Friction { get; set; } = 0.7f;

    //=== Debug info ===========================================================

    public bool IsColliding { get; set; } = false;

    //==========================================================================

    public Vector2 LocalToWorldSpace(Vector2 localPoint)
    {
        var rotated = Vector2.Rotate(localPoint, Rotation);

        return Position + rotated;
    }

    public Vector2 WorldToLocalSpace(Vector2 worldPoint)
    {
        var translated = worldPoint - Position;
        return Vector2.Rotate(translated, -Rotation);
    }

    //==========================================================================

    public void AddForce(Vector2 force)
    {
        SumForces += force;
    }

    public void AddTorque(float torque)
    {
        SumTorque += torque;
    }

    public void ClearForces()
    {
        SumForces = Vector2.Zero;
    }

    public void ClearTorque()
    {
        SumTorque = 0;
    }

    public void ApplyImpulseLinear(Vector2 impulse)
    {
        if (IsStatic)
        {
            // If the body is static, we don't apply impulses to it
            return;
        }

        // Update the velocity based on the impulse and mass
        Velocity += impulse * InvMass;
    }

    public void ApplyImpulseAngular(float j) 
    {
        if (IsStatic) { return; }
        
        AngularVelocity += j* InvI;
    }

    public void ApplyImpulseAtPoint(Vector2 impulse, Vector2 r)
    {
        if (IsStatic)
        {
            // If the body is static, we don't apply impulses to it
            return;
        }

        // Update the velocity based on the impulse and mass
        Velocity += impulse * InvMass;
        AngularVelocity += r.Cross(impulse) * InvI;
    }

    public void IntegrateForces(float dt)
    {
        // If the body is static, don't do anything with it
        if (IsStatic) { return; }

        // Linear motion integration
        // Find the acceleration based on the forces that are being applied and the mass
        Acceleration = SumForces * InvMass;

        // Update velocity based on acceleration
        Velocity += Acceleration * dt;

        // Angular motion integration
        // Find the angular acceleration based on the torque that is being applied and the moment of inertia
        AngularAcceleration = SumTorque * InvI;

        // Integrate the angular acceleration to find the new angular velocity
        AngularVelocity += AngularAcceleration * dt;

        ClearForces();
        ClearTorque();
    }

    public void IntegrateVelocities(float dt)
    {
        // If the body is static, don't do anything with it
        if (IsStatic) { return; }

        // Update position based on velocity
        Position += Velocity * dt;

        // Integrate the angular velocity to find the new rotation angle
        Rotation += AngularVelocity * dt;

        if (Rotation > MathHelper.TwoPi) { Rotation -= MathHelper.TwoPi; }
        if (Rotation < 0) { Rotation += MathHelper.TwoPi; }

        Shape.UpdateVertices(Rotation, Position);
    }
}
