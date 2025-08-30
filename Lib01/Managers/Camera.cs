using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers;

public static class Camera
{
    static Camera()
    {
        CalculateViewMatrix();
        CalculateProjectionMatrix();
    }

    private static float _zoom = 1.0f;
    private static Vector2 _origin = Vector2.Zero;
    private static Matrix _viewMatrix;
    private static Matrix _projectionMatrix;

    private static float _viewportWidth = 1600f;
    private static float _viewportHeight = 900f;
    private static float _nearPlane = 0f;
    private static float _farPlane = 1f;

    public static float Zoom
    {
        get => _zoom;
        set
        {
            if (_zoom != value)
            {
                _zoom = value;
                CalculateAndRaiseEvent();
            }
        }
    }

    public static Vector2 Origin
    {
        get => _origin;
        set
        {
            if (_origin != value)
            {
                _origin = value;
                CalculateAndRaiseEvent();
            }
        }
    }

    public static EventHandler<CameraChangedEventArgs> OnChanged;

    private static void CalculateViewMatrix()
    {
        _viewMatrix =
            Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f) *
            Matrix.CreateScale(Zoom, Zoom, 1f) *
            Matrix.CreateTranslation(Origin.X, Origin.Y, 0f);
    }

    private static void CalculateProjectionMatrix()
    {
        _projectionMatrix = Matrix.CreateOrthographicOffCenter(
            0, _viewportWidth,     // left, right
            _viewportHeight, 0,    // bottom, top (Y is inverted in MonoGame)
            _nearPlane, _farPlane  // near, far
        );
    }

    public static Matrix ViewMatrix => _viewMatrix;

    public static Matrix ProjectionMatrix => _projectionMatrix;

    public static void SetViewport(int width, int height)
    {
        _viewportWidth = width;
        _viewportHeight = height;

        CalculateAndRaiseEvent();
    }

    private static void CalculateAndRaiseEvent()
    {
        CalculateViewMatrix();
        CalculateViewMatrix();

        OnChanged?.Invoke(null, new CameraChangedEventArgs(_viewMatrix, _projectionMatrix));
    }
}
