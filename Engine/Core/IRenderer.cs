using System.Numerics;

namespace EngineLib.Core
{
    public interface IRenderer : IComponent
    {
        public void Render(Graphics graphics, Matrix3x2 transform, Viewport viewport);
    }

}
