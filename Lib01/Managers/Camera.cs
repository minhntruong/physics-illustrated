using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers;

public static class Camera
{
    public static void Initialize(Game game)
    {
        (game as IGameExt).WindowClientSizeChanged += (sender, args) => OnViewportChange();
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

    // Origin is the top-left corner of the viewport in world coordinates (pre-zoom)
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

    public static void SetZoomFocus(float zoomInc, Vector2 focusScreenPos)
    {
        // The goal is to:
        // 1) Change the zoom level
        // 2) Adjust the origin so that the world position under the cursor stays fixed at the new zoom level

        var worldPos = focusScreenPos / _zoom + Origin;

        _zoom += zoomInc;

        // Adjust Origin so the world position under the curs

        // TODO: This is not stable if you pan before zooming. Fix it.

        _origin = worldPos - (worldPos / _zoom);

        CalculateAndRaiseEvent();
    }

    public static EventHandler<CameraChangedEventArgs> OnChanged;

    private static void CalculateViewMatrix()
    {
        //_viewMatrix =
        //    //Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f) *
        //    Matrix.CreateScale(Zoom, Zoom, 1f) *
        //    Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f);
        _viewMatrix =
            Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f) *
            Matrix.CreateScale(Zoom, Zoom, 1f);
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

    //==========================================================================
    private static void OnViewportChange()
    {
        CalculateAndRaiseEvent();
    }

    private static void CalculateAndRaiseEvent()
    {
        CalculateViewMatrix();
        CalculateProjectionMatrix();

        OnChanged?.Invoke(null, new CameraChangedEventArgs(_viewMatrix, _projectionMatrix));
    }
}
