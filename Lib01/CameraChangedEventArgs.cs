using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library;

public class CameraChangedEventArgs : EventArgs
{
    public CameraChangedEventArgs(Matrix view, Matrix projection)
    {
        View = view;
        Projection = projection;
    }

    public Matrix View {  get; }

    public Matrix Projection { get; }

}
