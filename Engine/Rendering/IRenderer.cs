using EngineLib.Core;

namespace EngineLib.Rendering
{
    public interface IRenderer : IComponent
    {
        public void Render(Graphics graphics, Camera camera, Transform transform);
    }





}
