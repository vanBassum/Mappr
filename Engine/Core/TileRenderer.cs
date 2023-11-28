using System.Numerics;

namespace EngineLib.Core
{
    public class TileRenderer : IRenderer
    {
        public GameEntity Entity { get; set; } = GameEntity.Empty;
        public void Render(Graphics graphics, Matrix3x2 transform, Viewport viewport)
        {
            // TODO
        }
    }



}
