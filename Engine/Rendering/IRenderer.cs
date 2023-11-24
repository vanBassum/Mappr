using EngineLib.Core;

namespace EngineLib.Rendering
{
    public interface IRenderer : IComponent
    {
        public void Render(V2Graphics graphics, Camera camera, GameObject gameObject);
    }
}
