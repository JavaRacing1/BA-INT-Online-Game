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

        [ExportGroup("Limit")]
        // Maximum pixel offsets of the camera to the terrain, used for calculating the camera limits
        [Export(PropertyHint.Range, "0,10000,")] private int _cameraLimitOffsetX = 200;
        [Export(PropertyHint.Range, "0,10000,")] private int _cameraLimitOffsetY = 80;

        [ExportGroup("Zoom")]
        [Export(PropertyHint.Range, "0,1,")] private float _zoomSize = 0.1f;
        [Export(PropertyHint.Range, "0.01,2,")] private float _minZoom = 0.1f;
        [Export(PropertyHint.Range, "0.01,2,")] private float _maxZoom = 2f;

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
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void _Process(double delta)
        {
            Vector2I moveVector = Vector2I.Zero;
            if (Input.IsActionPressed("camera_left"))
            {
                moveVector.X -= 1;
            }

            if (Input.IsActionPressed("camera_up"))
            {
                moveVector.Y -= 1;
            }

            if (Input.IsActionPressed("camera_right"))
            {
                moveVector.X += 1;
            }

            if (Input.IsActionPressed("camera_down"))
            {
                moveVector.Y += 1;
            }

            Position += moveVector * _cameraSpeed;
            if (moveVector != Vector2I.Zero)
            {
                LimitPosition();
            }
        }

        /// <summary>
        /// Called when an InputEvent occurs
        /// </summary>
        /// <param name="event">The input event</param>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                int zoomDirectionMultiplier = mouseEvent.ButtonIndex switch
                {
                    MouseButton.WheelUp => 1,
                    MouseButton.WheelDown => -1,
                    _ => 0
                };

                if (zoomDirectionMultiplier != 0)
                {
                    float newZoom = Math.Clamp(Zoom.X + (_zoomSize * zoomDirectionMultiplier), _minZoom, _maxZoom);
                    Zoom = new Vector2(newZoom, newZoom);
                }
            }
        }

        private void LimitPosition()
        {
            float halfViewportX = GetViewportRect().Size.X / 2 * (1 / Zoom.X);
            float halfViewportY = GetViewportRect().Size.Y / 2 * (1 / Zoom.Y);

            GD.Print((GetViewportRect().Size.X / 2) + " " + (GetViewportRect().Size.Y / 2));
            GD.Print(halfViewportX + " " + halfViewportY);

            float limitedX = Math.Clamp(Position.X, LimitLeft + halfViewportX, LimitRight - halfViewportX);
            float limitedY = Math.Clamp(Position.Y, LimitTop + halfViewportY, LimitBottom - halfViewportY);
            Position = new Vector2(limitedX, limitedY);
        }
    }
}