using Godot;

namespace INTOnlineCoop.Script
{
    public partial class WaterTest : Node2D
    {
        [Export] private CanvasLayer _layer;
        [Export] private Viewport _renderViewport;
        [Export] private TextureRect _shaderRect;
        public override void _Ready()
        {
            if (_layer != null && _renderViewport != null && _shaderRect != null)
            {
                _layer.CustomViewport = _renderViewport;
                _shaderRect.Show();
            }
        }
    }
}