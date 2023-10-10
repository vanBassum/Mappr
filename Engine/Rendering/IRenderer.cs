using EngineLib.Core;

namespace EngineLib.Rendering
{
    public interface IRenderer : IComponent
    {
        void Render(V2Graphics g);
    }
}
