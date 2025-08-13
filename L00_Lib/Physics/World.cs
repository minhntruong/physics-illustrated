using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics.Constraints;
using PhysicsIllustrated.Library.Physics.Mathematics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static PhysicsIllustrated.Library.Physics.Constants;

namespace PhysicsIllustrated.Library.Physics;

public class World : IDisposable
{
    public World(float gravity)
    {
        Gravity = gravity;
    }

    private List<Body> _bodies = new List<Body>();
    private List<JointConstraint> _constraints = new List<JointConstraint>();
    private List<Vector2> _forces = new List<Vector2>();
    private List<float> _torques = new List<float>();
    private List<Contact> _contacts = new List<Contact>();

    private int _frame = 0;
    private bool _disposed;

    public float Gravity { get; private set; }

    #region IDisposable
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here if needed
            }

            // Free unmanaged resources here if needed

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    public List<Body> Bodies => _bodies;    
    public List<JointConstraint> Constraints => _constraints;
    public List<Contact> Contacts => _contacts;

    public void AddBody(Body body)
    {
        body.Id = _bodies.Count; // Assign a unique ID based on the current count
        _bodies.Add(body);
    }

    public void AddConstraint(ref JointConstraint constraint)
    {
        _constraints.Add(constraint);
    }

    public void AddContact(ref Contact contact)
    {
        _contacts.Add(contact);
    }

    public void AddForce(ref Vector2 force)
    {
        _forces.Add(force);
    }

    public void AddTorque(float torque)
    {
        _torques.Add(torque);
    }

    public void Update(float dt)
    {
        if (dt > 0) { _frame++; }
        else { return; }

        var penetrations = new List<PenetrationConstraint>();

        // Loop all bodies of the world applying forces

        foreach (var body in _bodies)
        {
            // Apply gravity
            var weight = new Vector2(0, Gravity * body.Mass * PIXELS_PER_METER);
            body.AddForce(weight);

            // Apply all forces
            foreach (var force in _forces)
            {
                body.AddForce(force);
            }

            // Apply all torques
            foreach (var torque in _torques)
            {
                body.AddTorque(torque);
            }
        }

        // Integrate all forces
        foreach (var body in _bodies)
        {
            body.IntegrateForces(dt);
        }

        //=== Check collisions =================================================

        for (var i = 0; i < _bodies.Count; i++)
        {
            for (var j = i + 1; j < _bodies.Count; j++)
            {
                var a = _bodies[i];
                var b = _bodies[j];

                var contacts = new List<Contact>();
                if (CollisionDetection.IsColliding(a, b, contacts))
                {
                    foreach (var contact in contacts)
                    {
                        // Create a new penetration constraint
                        var penetration = new PenetrationConstraint(contact.A, contact.B, contact.Start, contact.End, contact.Normal);

                        penetrations.Add(penetration);
                    }
                }

                Contacts.Clear();
                Contacts.AddRange(contacts);
            }
        }

        //=== Solve all constraints ============================================

        // PreSolve
        foreach (var constraint in _constraints)
        {
            constraint.PreSolve(dt);
        }
        foreach (var constraint in penetrations)
        {
            constraint.PreSolve(dt);
        }

        // Solve
        for (var i = 0; i < 5; i++)
        {
            // Solve all constraints
            foreach (var constraint in _constraints)
            {
                constraint.Solve();
            }
            foreach (var constraint in penetrations)
            {
                constraint.Solve();
            }
        }

        // PostSolve
        foreach (var constraint in _constraints)
        {
            constraint.PostSolve();
        }
        foreach (var constraint in penetrations)
        {
            constraint.PostSolve();
        }
        

        //======================================================================

        // Integrate all the velocities
        foreach (var body in _bodies)
        {
            body.IntegrateVelocities(dt);
        }
    }
}
