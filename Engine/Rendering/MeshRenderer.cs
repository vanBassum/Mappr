using EngineLib.Core;
using EngineLib.Extentions;
using System.Numerics;

namespace EngineLib.Rendering
{
    public class MeshRenderer : IRenderer
    {
        public Pen Pen { get; set; } = Pens.Black;
        public List<Mesh> Meshes { get; } = new List<Mesh>();

        public void Render(Graphics graphics, Camera camera, Transform transform)
        {
            foreach (var mesh in Meshes)
            {
                if (mesh.Vertices.Count == 0 || Pen == null)
                    continue;


                //var transformedPoints = mesh.Transform(transform).Vertices.Select(p => camera.Transform.InverseTransformPoint(p)).ToPoints().ToArray();
                Transform inverseCameraTransform = camera.Transform.Inverse();
                var transformedPoints = mesh.Transform(transform).Transform(inverseCameraTransform).Vertices.Select(p=>p + camera.ViewPort / 2).ToPoints().ToArray();
                graphics.DrawPolygon(Pen, transformedPoints);
            }
        }
    }





}
