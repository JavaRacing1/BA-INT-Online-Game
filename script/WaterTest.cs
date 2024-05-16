using Godot;

using INTOnlineCoop.Script.Level;

namespace INTOnlineCoop.Script
{
    public partial class WaterTest : Node2D
    {
        [Export] private CanvasLayer _layer;
        [Export] private Viewport _renderViewport;
        [Export] private TextureRect _shaderRect;
        [Export] private PlayerCamera _camera;
        public override void _Ready()
        {
            if (_camera != null)
            {
                _camera.Init(new Vector2I(10000, 7000));
            }
            if (_layer != null && _renderViewport != null && _shaderRect != null)
            {
                _layer.CustomViewport = _renderViewport;
                _shaderRect.Show();
            }
        }
    }
}