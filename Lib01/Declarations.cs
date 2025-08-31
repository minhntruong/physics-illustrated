using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library;

public interface IGameExt
{
    event EventHandler<EventArgs> WindowClientSizeChanged;

    void RaiseWindowClientSizeChanged();
}

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
