using EngineLib.Core;
using System.Numerics;

namespace EngineLib.Rendering
{
    public interface ICollider : IComponent
    {
        public bool Collides(Vector2 worldPosition, Transform transform);
    }





}
