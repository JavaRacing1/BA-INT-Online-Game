using System;

using Godot;

namespace INTOnlineCoop.Script.Level
{
    /// <summary>
    /// Camera of the player
    /// </summary>
    public partial class PlayerCamera : Camera2D
    {
        [Export(PropertyHint.Range, "0,500,")] private int _cameraSpeed = 10;
        [Export(PropertyHint.Range, "0,200,")] private int _mouseMoveDistance = 20;

        [ExportGroup("Limit")]
        // Maximum pixel offsets of the camera to the terrain, used for calculating the camera limits
        [Export(PropertyHint.Range, "0,10000,")] private int _cameraLimitOffsetX = 300;
        [Export(PropertyHint.Range, "0,10000,")] private int _cameraLimitOffsetY = 120;

        [ExportGroup("Zoom")]
        [Export(PropertyHint.Range, "0,1,")] private float _zoomSize = 0.3f;
        [Export(PropertyHint.Range, "0.01,2,")] private float _minZoom = 0.1f;
        [Export(PropertyHint.Range, "0.01,2,")] private float _maxZoom = 2f;

        private const float ZoomInterpolationWeight = 0.1f;
        private bool _isZoomingOut;
        private bool _isZoomingIn;
        private bool _windowIsFocused = true;

        /// <summary>
        /// Initializes the camera
        /// </summary>
        /// <param name="terrainSize">Size of the terrain</param>
        public void Init(Vector2I terrainSize)
        {
            LimitLeft = -_cameraLimitOffsetX;
            LimitTop = -_cameraLimitOffsetY * 2;
            LimitRight = terrainSize.X + _cameraLimitOffsetX;
            LimitBottom = terrainSize.Y + _cameraLimitOffsetY;

            Position = new Vector2(terrainSize.X / 2f, terrainSize.Y / 2f);
            Zoom = new Vector2(0.1f, 0.1f);
        }

        /// <summary>
        /// Moves the camera to a position
        /// </summary>
        /// <param name="newPosition">New position of the camera</param>
        public void MoveCamera(Vector2 newPosition)
        {
            Position = newPosition;
            LimitPosition();
        }

        /// <summary>
        /// Changes the zoom level of the camera
        /// </summary>
        /// <param name="newZoomLevel">New zoom level</param>
        public void ChangeCameraZoom(float newZoomLevel)
        {
            float clampedZoom = Math.Clamp(newZoomLevel, _minZoom, _maxZoom);
            Zoom = new Vector2(clampedZoom, clampedZoom);
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override async void _Process(double delta)
        {
            if (GameLevel.IsInputBlocked)
            {
                return;
            }
            Vector2 mousePosition = GetViewport().GetMousePosition();
            Vector2I moveVector = Vector2I.Zero;
            if (Input.IsActionPressed("camera_left") ||
                (mousePosition.X <= _mouseMoveDistance && IsMouseInsideWindow()))
            {
                moveVector.X -= 1;
            }

            if (Input.IsActionPressed("camera_up") || (mousePosition.Y <= _mouseMoveDistance && IsMouseInsideWindow()))
            {
                moveVector.Y -= 1;
            }

            if (Input.IsActionPressed("camera_right") ||
                (mousePosition.X >= GetViewportRect().Size.X - _mouseMoveDistance && IsMouseInsideWindow()))
            {
                moveVector.X += 1;
            }

            if (Input.IsActionPressed("camera_down") ||
                (mousePosition.Y >= GetViewportRect().Size.Y - _mouseMoveDistance && IsMouseInsideWindow()))
            {
                moveVector.Y += 1;
            }

            if (!_windowIsFocused && moveVector != Vector2I.Zero)
            {
                moveVector = Vector2I.Zero;
            }

            Position += moveVector * (int)(_cameraSpeed * (1 / Zoom.X));
            if (moveVector != Vector2I.Zero)
            {
                PositionSmoothingEnabled = true;
                LimitPosition();
            }

            if (_isZoomingIn || _isZoomingOut)
            {
                float newZoom = _isZoomingIn ? Zoom.X + _zoomSize : Zoom.X - _zoomSize;
                float interpolatedZoom = Mathf.Lerp(Zoom.X, Math.Clamp(newZoom, _minZoom, _maxZoom), ZoomInterpolationWeight);
                Zoom = new Vector2(interpolatedZoom, interpolatedZoom);
                _ = await ToSignal(GetTree().CreateTimer(0.05), Timer.SignalName.Timeout);
                if (_isZoomingIn)
                {
                    _isZoomingIn = false;
                }

                if (_isZoomingOut)
                {
                    _isZoomingOut = false;
                }
            }
        }

        /// <summary>
        /// Called when an InputEvent occurs
        /// </summary>
        /// <param name="event">The input event</param>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (GameLevel.IsInputBlocked)
            {
                return;
            }
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                PositionSmoothingEnabled = false;
                switch (mouseButtonEvent.ButtonIndex)
                {
                    case MouseButton.WheelUp: _isZoomingIn = true; break;
                    case MouseButton.WheelDown: _isZoomingOut = true; break;
                }
            }
        }

        public override void _Notification(int what)
        {
            _windowIsFocused = (long)what switch
            {
                MainLoop.NotificationApplicationFocusIn => true,
                MainLoop.NotificationApplicationFocusOut => false,
                _ => _windowIsFocused
            };
        }

        private void LimitPosition()
        {
            float halfViewportX = GetViewportRect().Size.X / 2 * (1 / Zoom.X);
            float halfViewportY = GetViewportRect().Size.Y / 2 * (1 / Zoom.Y);
            float limitedX = Math.Clamp(Position.X, LimitLeft + halfViewportX, LimitRight - halfViewportX);
            float limitedY = Math.Clamp(Position.Y, LimitTop + halfViewportY, LimitBottom - halfViewportY);
            Position = new Vector2(limitedX, limitedY);
        }

        private bool IsMouseInsideWindow()
        {
            Vector2 mousePosition = GetViewport().GetMousePosition();
            Vector2 windowSize = GetViewportRect().Size;
            return 0 <= mousePosition.X && mousePosition.X <= windowSize.X && 0 <= mousePosition.Y &&
                   mousePosition.Y <= windowSize.Y;
        }
    }
}