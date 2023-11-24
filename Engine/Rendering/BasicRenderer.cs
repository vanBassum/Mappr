using EngineLib.Core;

namespace EngineLib.Rendering
{

    public class MeshRenderer : IRenderer
    {
        public Mesh Mesh { get; set; } = new Mesh();

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
