using EngineLib.Core;

namespace EngineLib.Rendering
{
    public class BasicRenderer : IRenderer
    {
        private readonly Action<V2Graphics> _render;
        public BasicRenderer(Action<V2Graphics> render)
        {
            _render = render;
        }

        public void Render(V2Graphics g)
        {
            _render.Invoke(g);
        }

        public void Update()
        {
            // Implement any update logic for rendering component
        }
    }
}
